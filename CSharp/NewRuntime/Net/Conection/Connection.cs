using UselessFrame.Net;
using System.Net.Sockets;
using System;
using System.Threading;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private Guid _guid;
        private TcpClient _client;
        private ByteBufferPool _pool;
        private ConnectionState _state;
        private CancellationTokenSource _closeTokenSource;

        public Guid Id => _guid;

        public Connection(Guid guid, TcpClient client)
        {
            _guid = guid;
            _client = client;
            _pool = new ByteBufferPool();
            _state = ConnectionState.Normal;
            _closeTokenSource = new CancellationTokenSource();
            Console.WriteLine($"connect client success");
            RequestMessage().Forget();
        }

        public void Close()
        {
            _state = ConnectionState.NormalClose;
            _closeTokenSource.Cancel();
            _client.Close();
            _client.Dispose();
            _pool.Dispose();
            _client = null;
            _pool = null;
        }
    }
}
