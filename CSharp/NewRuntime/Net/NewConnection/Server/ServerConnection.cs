
using System;
using System.Net.Sockets;
using TestIMGUI.Core;
using UselessFrame.Net;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        private Guid _id;
        private ConnectionFsm _fsm;
        private TcpClient _client;
        private MessageStream _stream;
        private ByteBufferPool _pool;
        private Action<MessageResult> _onReceiveMessage;

        public event Action<MessageResult> ReceiveMessageEvent
        {
            add { _onReceiveMessage += value; }
            remove { _onReceiveMessage -= value; }
        }

        public ServerConnection(Guid id, TcpClient client)
        {
            _id = id;
            _client = client;
            _stream = new MessageStream(this);
            _fsm = new ConnectionFsm(this, new ConnectionState[]
            {
                new VerifyTokenState()
            });
            _fsm.Start<VerifyTokenState>();
        }
    }
}
