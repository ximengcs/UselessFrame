
using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using UselessFrame.Net;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        private ConnectionFsm _fsm;
        private TcpClient _client;
        private MessageStream _stream;
        private ByteBufferPool _pool;
        private Guid _id;

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
