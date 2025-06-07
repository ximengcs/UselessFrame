using System;
using UselessFrame.Net;
using System.Threading;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Observable;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private Guid _guid;
        private TcpClient _client;
        private ByteBufferPool _pool;
        private Subject<Connection, ConnectionState> _state;
        private CancellationTokenSource _closeTokenSource;

        public Action<string> OnReceiveMessage;

        public Guid Id => _guid;

        public Subject<Connection, ConnectionState> State => _state;

        public Connection(Guid guid, TcpClient client)
        {
            _guid = guid;
            _client = client;
            _pool = new ByteBufferPool();
            _state = new Subject<Connection, ConnectionState>(this, ConnectionState.Normal);
            _closeTokenSource = new CancellationTokenSource();
            Console.WriteLine($"connect client success");
            RequestMessage().Forget();
        }

        public async UniTask Close()
        {
            _state.Value = ConnectionState.NormalClose;
            _closeTokenSource.Cancel();

            WriteMessageResult result = await MessageUtility.WriteCloseMessageAsync(_client);
            if (result.State != NetOperateState.OK)
            {
                Console.WriteLine($"notify server close error happen.");
            }
            else
            {
                Console.WriteLine($"notify server close.");
            }

            Dispose();
        }

        private void Dispose()
        {
            _client.Close();
            _client.Dispose();
            _pool.Dispose();
            _client = null;
            _pool = null;
        }
    }
}
