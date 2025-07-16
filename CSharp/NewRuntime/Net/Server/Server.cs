
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using IdGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    internal partial class Server : IServer, INetStateTrigger
    {
        private bool _disposed;
        private bool _start;
        private long _id;
        private IPEndPoint _host;
        private TcpListener _listener;
        private NetFsm<Server> _fsm;
        private IFiber _fiber;
        private Dictionary<long, Connection> _connections;
        private ISubject<IServer, ServerState> _state;
        private Action<IConnection> _onConnectionListChange;
        private IdGenerator _idGenerator;

        public long Id => _id;

        public IPEndPoint Host => _host;

        public ISubject<IServer, ServerState> State => _state;

        public string GetDebugPrefix<T>(NetFsmState<T> state) where T : INetStateTrigger
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[Host:{_host}]");
            sb.Append($"[{state.GetType().Name,-18}]");
            return sb.ToString();
        }


        public event Action<IConnection> NewConnectionEvent
        {
            add { _onConnectionListChange += value; }
            remove { _onConnectionListChange -= value; }
        }

        public IReadOnlyList<IConnection> Connections => new List<IConnection>(_connections.Values);

        public Server(long id, int port, IFiber fiber)
        {
            _fiber = fiber;
            _disposed = false;
            _id = id;
            _idGenerator = new IdGenerator(0, new IdGeneratorOptions(timeSource: new TimeTicksSource()));
            _state = new ValueSubject<IServer, ServerState>(this, fiber, ServerState.None);
            _connections = new Dictionary<long, Connection>();
            _host = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            _listener = new TcpListener(_host);
            _fsm = new NetFsm<Server>(this, new Dictionary<Type, NetFsmState<Server>>()
            {
                { typeof(StartState), new StartState() },
                { typeof(ListenState), new ListenState() },
                { typeof(CloseState), new CloseState() },
                { typeof(DisposeState), new DisposeState() }
            });
        }

        public IConnection GetConnection(long id)
        {
            if (_connections.TryGetValue(id, out var connection)) return connection;
            return null;
        }

        public void Start()
        {
            if (_start) return;
            _start = true;
            _fsm.Start<StartState>();
        }

        public void Close()
        {
            if (_disposed) return;
            _disposed = true;
            _fsm.Current.ChangeState<CloseState>().Forget();
        }

        internal void Dispose()
        {
            _listener = null;
            _connections = null;
            _onConnectionListChange = null;
            _fsm = null;
            _fiber = null;
            X.UnRegisterServer(this);
        }

        public void RemoveConnection(IConnection connection)
        {
            if (_connections.ContainsKey(connection.Id))
            {
                _connections.Remove(connection.Id);
                X.SystemLog.Debug($"[Server][Host:{_host}]remove a connectin, current count : {_connections.Count}");
            }
        }

        public void AddConnection(Connection connection)
        {
            _connections.Add(connection.Id, connection);
            X.SystemLog.Debug($"[Server][Host:{_host}]add new connectin, current count : {_connections.Count}");
            _onConnectionListChange?.Invoke(connection);
        }

        public void Broadcast(IMessage message)
        {
            foreach (var entry in _connections)
            {
                entry.Value.Send(message, true).Forget();
            }
        }

        public void TriggerState(int newState)
        {
            _state.Value = (ServerState)newState;
        }

        public void CancelAllAsyncWait()
        {
        }
    }
}
