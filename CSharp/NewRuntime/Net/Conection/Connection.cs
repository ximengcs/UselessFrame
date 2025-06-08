using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UselessFrame.Net;
using UselessFrame.Runtime.Observable;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private Guid _guid;
        private TcpClient _client;
        private IPEndPoint _ip;
        private ByteBufferPool _pool;
        private Subject<Connection, ConnectionState> _state;
        private CancellationTokenSource _closeTokenSource;

        public Action<IMessage> OnReceiveMessage;

        public IPEndPoint RemoteIP => _ip;

        public IPEndPoint LocalIP => (IPEndPoint)_client.Client.LocalEndPoint;

        public Guid Id => _guid;

        public Subject<Connection, ConnectionState> State => _state;

        public Connection(Guid guid, TcpClient client)
        {
            _guid = guid;
            _client = client;
            _ip = (IPEndPoint)client.Client.RemoteEndPoint;
            _pool = new ByteBufferPool();
            _state = new Subject<Connection, ConnectionState>(this, ConnectionState.Normal);
            _closeTokenSource = new CancellationTokenSource();
            Console.WriteLine($"connect success target {_ip.Address}:{_ip.Port}");
            RequestMessage().Forget();
        }

        public Connection(IPEndPoint ip)
        {
            _ip = ip;
            _guid = Guid.Empty;
            _pool = new ByteBufferPool();
            _state = new Subject<Connection, ConnectionState>(this, ConnectionState.None);
            _closeTokenSource = new CancellationTokenSource();
            Connect().Forget();
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
            _ip = null;
        }
    }
}
