
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Net;
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

        public async UniTask Send(IMessage message, bool autoRelease)
        {
            await _fsm.Current.OnSendMessage(message, _dataFiber);
            if (autoRelease)
                NetPoolUtility.ReleaseMessage(message);
        }

        public async UniTask<MessageResult> SendWait(IMessage message, bool autoRelease)
        {
            MessageResult result = await _fsm.Current.OnSendWaitMessage(message, _dataFiber);
            if (autoRelease)
                NetPoolUtility.ReleaseMessage(message);
            return result;
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
