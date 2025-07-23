
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private long _id;
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
        private CancellationTokenSource _disposeTokenSource;
        private long _serverTimeGap;

        internal Action<IConnection> OnDestroy;

        internal NetFsm<Connection> Fsm => _fsm;

        internal MessageStream Stream => _stream;

        public DateTime RemoteTime => DateTime.UtcNow.AddTicks(_serverTimeGap);

        public long Id => _id;

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
            if (_id != 0)
                sb.Append($"[ID:{_id}]");
            if (_localIP != null)
                sb.Append($"[L:{_localIP}]");
            else
                sb.Append("[L:Waiting]");
            sb.Append($"[R:{_remoteIP}]");
            sb.Append($"[{state.GetType().Name,-18}]");
            return sb.ToString();
        }

        public Connection(Server server, long id, TcpClient client, IFiber fiber)
        {
            _disposeTokenSource = new CancellationTokenSource();
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
            _stream = new MessageStream(this, _disposeTokenSource.Token);
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
            _disposeTokenSource = new CancellationTokenSource();
            _runtimeData = new Dictionary<Type, object>()
            {
                { typeof(ConnectionSetting), new ConnectionSetting() }
            };
            _id = 0L;
            _dataFiber = fiber;
            _pool = new ByteBufferPool();
            _state = new ValueSubject<IConnection, ConnectionState>(this, fiber, ConnectionState.None);
            _client = new TcpClient(AddressFamily.InterNetwork);
            _remoteIP = remoteIP;
            _stream = new MessageStream(this, _disposeTokenSource.Token);
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
            _disposeTokenSource.Cancel();
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

        public void ForceClose()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;
            _runFiber.Post(ToFiberFun.CloseOnFiber, _fsm);
        }

        public void Close()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;
            _runFiber.Post(ToFiberFun.CloseOnFiber, _fsm);
        }

        public T GetRuntimeData<T>()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return default;
            if (_runtimeData.TryGetValue(typeof(T), out var data))
            {
                return (T)data;
            }
            return default(T);
        }

        public async UniTask Send(IMessage message, bool autoRelease)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            AutoResetUniTaskCompletionSource completeSource = AutoResetUniTaskCompletionSource.Create();
            _runFiber.Post(ToFiberFun.SendMessageToRunFiber, Tuple.Create(this, message, autoRelease, completeSource));
            await completeSource.Task;
        }

        public async UniTask<MessageResult> SendWait(IMessage message, bool autoRelease)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return default(MessageResult);
            AutoResetUniTaskCompletionSource<MessageResult> completeSource = AutoResetUniTaskCompletionSource<MessageResult>.Create();
            _runFiber.Post(ToFiberFun.SendWaitMessageToRunFiber, Tuple.Create(this, message, autoRelease, completeSource));
            return await completeSource.Task;
        }

        public async UniTask<LatencyResult> TestLatency()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return default(LatencyResult);
            long time = DateTime.UtcNow.Ticks;
            TestLatencyMessage msg = NetPoolUtility.CreateMessage<TestLatencyMessage>();
            MessageResult result = await SendWait(msg, true);
            if (result.Valid)
            {
                TestLatencyResponseMessage rspMsg = (TestLatencyResponseMessage)result.Message;
                long nowTicks = DateTime.UtcNow.Ticks;
                LatencyResult latency = new LatencyResult()
                {
                    Success = true,
                    DeltaTime = (int)((nowTicks - time) / TimeSpan.TicksPerMillisecond)
                };
                return latency;
            }
            else
            {
                LatencyResult latency = new LatencyResult()
                {
                    Success = false
                };
                return latency;
            }
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
