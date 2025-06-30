
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    internal partial class Connection : IConnection, INetStateTrigger
    {
        private Guid _id;
        private IFiber _runFiber;
        private IFiber _dataFiber;
        private TcpClient _client;
        private IPEndPoint _localIP;
        private IPEndPoint _remoteIP;
        private ByteBufferPool _pool;
        private MessageStream _stream;
        private NetFsm<Connection> _fsm;
        private Server _server;
        private ISubject<IConnection, ConnectionState> _state;
        private Action<MessageResult> _onReceiveMessage;
        private Dictionary<Type, object> _runtimeData;

        internal NetFsm<Connection> Fsm => _fsm;

        internal MessageStream Stream => _stream;

        public Guid Id => _id;

        public IPEndPoint LocalIP => _localIP;

        public IPEndPoint RemoteIP => _remoteIP;

        public ISubject<IConnection, ConnectionState> State => _state;

        public event Action<MessageResult> ReceiveMessageEvent
        {
            add { _onReceiveMessage += value; }
            remove { _onReceiveMessage -= value; }
        }

        public string GetDebugPrefix<T>(NetFsmState<T> state) where T : INetStateTrigger
        {
            StringBuilder sb = new StringBuilder();
            if (_id != Guid.Empty)
                sb.Append($"[ID:{_id}]");
            if (_localIP != null)
                sb.Append($"[L:{_localIP}]");
            else
                sb.Append("[L:Waiting]");
            sb.Append($"[R:{_remoteIP}]");
            sb.Append($"[{state.GetType().Name,-18}]");
            return sb.ToString();
        }

        public Connection(Server server, Guid id, TcpClient client, IFiber fiber)
        {
            _runtimeData = new Dictionary<Type, object>()
            {
                { typeof(ConnectionSetting), new ConnectionSetting() }
            };
            _id = id;
            _server = server;
            _client = client;
            _dataFiber = fiber;
            _pool = new ByteBufferPool();
            _state = new ValueSubject<IConnection, ConnectionState>(this, fiber, ConnectionState.None);
            _localIP = (IPEndPoint)client.Client.LocalEndPoint;
            _remoteIP = (IPEndPoint)client.Client.RemoteEndPoint;
            _stream = new MessageStream(this);
            _fsm = new NetFsm<Connection>(this, new Dictionary<Type, NetFsmState<Connection>>
            {
                { typeof(ConnectState), new ConnectState() },
                { typeof(CheckConnectState), new CheckConnectState() },
                { typeof(CloseRequestState), new CloseRequestState() },
                { typeof(CloseResponseState), new CloseResponseState() },
                { typeof(DisposeState), new DisposeState() },
                { typeof(RunState), new RunState() },
                { typeof(TokenCheck), new TokenVerifyState() }

            });
            _runFiber = X.FiberManager.Create();
            _runFiber.Post(ToFiberFun.RunCheckOnFiber, _fsm);
        }

        public Connection(IPEndPoint remoteIP, IFiber fiber)
        {
            _runtimeData = new Dictionary<Type, object>()
            {
                { typeof(ConnectionSetting), new ConnectionSetting() }
            };
            _id = Guid.Empty;
            _dataFiber = fiber;
            _pool = new ByteBufferPool();
            _state = new ValueSubject<IConnection, ConnectionState>(this, fiber, ConnectionState.None);
            _client = new TcpClient(AddressFamily.InterNetwork);
            _remoteIP = remoteIP;
            _stream = new MessageStream(this);
            _fsm = new NetFsm<Connection>(this, new Dictionary<Type, NetFsmState<Connection>>
            {
                { typeof(ConnectState), new ConnectState() },
                { typeof(CheckConnectState), new CheckConnectState() },
                { typeof(CloseRequestState), new CloseRequestState() },
                { typeof(CloseResponseState), new CloseResponseState() },
                { typeof(DisposeState), new DisposeState() },
                { typeof(RunState), new RunState() },
                { typeof(TokenCheck), new TokenResponseState() }

            });
            _runFiber = X.FiberManager.Create();
            _runFiber.Post(ToFiberFun.RunConnectOnFiber, _fsm);
        }

        public void Dispose()
        {
            _onReceiveMessage = null;
            _fsm.Dispose();
            _stream.Dispose();
            _client.Dispose();
            _pool.Dispose();
            _runFiber.Dispose();

            _runtimeData = null;
            _localIP = null;
            _remoteIP = null;
            _fsm = null;
            _stream = null;
            _client = null;
            _pool = null;
            _runFiber = null;
            if (_server != null)
            {
                _dataFiber.Post(ToFiberFun.UnRegisteConnection, Tuple.Create(_server, this));
            }
        }

        public void Close()
        {
            _fsm.ChangeState(typeof(CloseRequestState)).Forget();
        }

        public T GetRuntimeData<T>()
        {
            if (_runtimeData.TryGetValue(typeof(T), out var data))
            {
                return (T)data;
            }
            return default(T);
        }

        public async UniTask Send(IMessage message, bool autoRelease)
        {
            AutoResetUniTaskCompletionSource completeSource = AutoResetUniTaskCompletionSource.Create();
            _runFiber.Post(ToFiberFun.SendMessageToRunFiber, Tuple.Create(this, message, autoRelease, completeSource));
            await completeSource.Task;
        }

        public async UniTask<MessageResult> SendWait(IMessage message, bool autoRelease)
        {
            AutoResetUniTaskCompletionSource<MessageResult> completeSource = AutoResetUniTaskCompletionSource<MessageResult>.Create();
            _runFiber.Post(ToFiberFun.SendWaitMessageToRunFiber, Tuple.Create(this, message, autoRelease, completeSource));
            return await completeSource.Task;
        }

        public void TriggerState(int newState)
        {
            _state.Value = (ConnectionState)newState;
        }

        public void TriggerNewMessage(MessageResult message)
        {
            if (_onReceiveMessage == null)
                return;

            _dataFiber.Post(ToFiberFun.TriggerMessageToDataFiber, Tuple.Create(_onReceiveMessage, message));
        }

        public void CancelAllAsyncWait()
        {
            _stream.CancelAllWait();
        }
    }
}
