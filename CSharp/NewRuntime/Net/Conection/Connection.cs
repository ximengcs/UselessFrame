
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
using UselessFrame.Runtime.Pools;

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
        private ISubject<IConnection, ConnectionState> _state;
        private Action<IMessageResult> _onReceiveMessage;

        internal NetFsm<Connection> Fsm => _fsm;

        internal MessageStream Stream => _stream;

        public Guid Id => _id;

        public IPEndPoint LocalIP => _localIP;

        public IPEndPoint RemoteIP => _remoteIP;

        public ISubject<IConnection, ConnectionState> State => _state;

        public event Action<IMessageResult> ReceiveMessageEvent
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

        public Connection(Guid id, TcpClient client, IFiber fiber)
        {
            _id = id;
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
            _runFiber.Post(RunCheckOnFiber, null);
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
            _runFiber.Post(RunConnectOnFiber, null);
        }

        private void RunCheckOnFiber(object _)
        {
            _fsm.Start<CheckConnectState>();
        }

        private void RunConnectOnFiber(object _)
        {
            _fsm.Start<ConnectState>();
        }

        public void Send(IMessage message)
        {
            _fsm.Current.OnSendMessage(message).Forget();
        }

        public async UniTask<IMessageResult> SendWait(IMessage message)
        {
            return await _fsm.Current.OnSendWaitMessage(message);
        }

        public void TriggerState(int newState)
        {
            _state.Value = (ConnectionState)newState;
        }

        public void TriggerNewMessage(MessageResult message)
        {
            if (_onReceiveMessage == null)
                return;

            _dataFiber.Post(TriggerMessageToDataFiber, message);
        }

        private void TriggerMessageToDataFiber(object data)
        {
            IMessageResult result = (IMessageResult)data;
            _onReceiveMessage?.Invoke(result);
        }
    }
}
