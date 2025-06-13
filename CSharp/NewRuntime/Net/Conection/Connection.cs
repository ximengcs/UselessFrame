
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    internal partial class Connection : IConnection, INetStateTrigger
    {
        private Guid _id;
        private TcpClient _client;
        private IPEndPoint _localIP;
        private IPEndPoint _remoteIP;
        private ByteBufferPool _pool;
        private MessageStream _stream;
        private NetFsm<Connection> _fsm;
        private Action<MessageResult> _onReceiveMessage;
        private Action<ConnectionState, ConnectionState> _onStateChange;

        public Guid Id => _id;

        public IPEndPoint LocalIP => _localIP;

        public IPEndPoint RemoteIP => _remoteIP;

        public event Action<MessageResult> ReceiveMessageEvent
        {
            add { _onReceiveMessage += value; }
            remove { _onReceiveMessage -= value; }
        }

        public event Action<ConnectionState, ConnectionState> StateChangeEvent
        {
            add { _onStateChange += value; }
            remove { _onStateChange -= value; }
        }

        public Connection(Guid id, TcpClient client)
        {
            _id = id;
            _client = client;
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
                { typeof(TokenVerifyState), new TokenVerifyState() }

            });
            _fsm.Start<TokenVerifyState>();
        }

        public Connection(IPEndPoint remoteIP)
        {
            _id = Guid.Empty;
            _client = new TcpClient(AddressFamily.InterNetwork);
            _localIP = (IPEndPoint)_client.Client.LocalEndPoint;
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
                { typeof(TokenResponseState), new TokenResponseState() }

            });
            _fsm.Start<ConnectState>();
        }

        public void Send(IMessage message)
        {
            _fsm.Current.OnSendMessage(message).Forget();
        }

        public async UniTask<MessageResult> SendWait(IMessage message)
        {
            return await _fsm.Current.OnSendWaitMessage(message);
        }

        public void TriggerState(int oldState, int newState)
        {
            _onStateChange?.Invoke((ConnectionState)oldState, (ConnectionState)newState);
        }
    }
}
