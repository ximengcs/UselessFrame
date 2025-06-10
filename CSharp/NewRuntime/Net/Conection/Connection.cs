using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.Runtime.Observable;

namespace TestIMGUI.Core
{
    /// <summary>
    /// 单个Connect只能在一个线程中
    /// </summary>
    public partial class Connection
    {
        private Guid _guid;
        private TcpClient _client;
        private IPEndPoint _ip;
        private ByteBufferPool _pool;
        private EventToFiberEnumSubject<Connection, ConnectionState> _state;
        private CancellationTokenSource _closeTokenSource;
        private IFiber _dataFiber;

        private bool _disposed;
        private readonly object _disposeLock = new object();

        public Action<MessageResult> OnReceiveMessage;

        public IPEndPoint RemoteIP => _ip;

        public IPEndPoint LocalIP
        {
            get
            {
                if (_client == null || _client.Client == null)
                    return new IPEndPoint(0, 0);
                return (IPEndPoint)_client.Client.LocalEndPoint;
            }
        }

        public Guid Id => _guid;

        public ISubject<Connection, ConnectionState> State => _state;

        public Connection(Guid guid, TcpClient client, IFiber dataFiber)
        {
            _guid = guid;
            _client = client;
            _dataFiber = dataFiber;
            _ip = (IPEndPoint)client.Client.RemoteEndPoint;
            _pool = new ByteBufferPool();
            _state = new EventToFiberEnumSubject<Connection, ConnectionState>(this, ConnectionState.TokenPending, dataFiber);
            _closeTokenSource = new CancellationTokenSource();
            RequestMessage().Forget();
        }

        public Connection(IPEndPoint ip, IFiber dataFiber)
        {
            _ip = ip;
            _guid = Guid.Empty;
            _dataFiber = dataFiber;
            _pool = new ByteBufferPool();
            _state = new EventToFiberEnumSubject<Connection, ConnectionState>(this, ConnectionState.None, dataFiber);
            _closeTokenSource = new CancellationTokenSource();
            _client = new TcpClient(AddressFamily.InterNetwork);
            Connect().Forget();
        }

        internal void Start()
        {
            _state.Value = ConnectionState.Normal;
            X.SystemLog.Debug("Net", $"connect success target {Id} {_ip.Address}:{_ip.Port} {_state.Value}");
        }

        public async UniTask Close()
        {
            lock (_disposeLock)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }

            _state.Value = ConnectionState.NormalClose;
            _closeTokenSource.Cancel();
            WriteMessageResult result = await MessageUtility.WriteCloseMessageAsync(_client);
            if (result.State != NetOperateState.OK)
            {
                X.SystemLog.Debug("Net", $" {Id} notify server close error happen. {result.StateMessage}");
            }
            else
            {
                X.SystemLog.Debug("Net", $" {Id} notify server close.");
            }

            Dispose();
        }

        internal void InnerClose()
        {
            lock (_disposeLock)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }

            X.SystemLog.Debug("Net", $" {Id} will inner close.");
            _closeTokenSource.Cancel();
            Dispose();
        }

        private void Dispose()
        {
            _waitResponseList = null;
            OnReceiveMessage = null;
            _client.Close();
            _pool.Dispose();
            _pool = null;
        }
    }
}
