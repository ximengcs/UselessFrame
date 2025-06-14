
using System;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    internal partial class Server : IServer, INetStateTrigger
    {
        private bool _disposed;
        private bool _start;
        private IPEndPoint _host;
        private TcpListener _listener;
        private NetFsm<Server> _fsm;
        private IFiber _fiber;
        private Dictionary<Guid, Connection> _connections;
        private ISubject<IServer, ServerState> _state;
        private Action<IConnection> _onConnectionListChange;

        public IPEndPoint Host => _host;

        public ISubject<IServer, ServerState> State => _state;


        public event Action<IConnection> NewConnectionEvent
        {
            add { _onConnectionListChange += value; }
            remove { _onConnectionListChange -= value; }
        }

        public IReadOnlyList<IConnection> Connections => new List<IConnection>(_connections.Values);

        public Server(int port, IFiber fiber)
        {
            _fiber = fiber;
            _disposed = false;
            _state = new ValueSubject<IServer, ServerState>(this, ServerState.None);
            _connections = new Dictionary<Guid, Connection>();
            _host = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            _listener = new TcpListener(_host);
            _fsm = new NetFsm<Server>(this, new Dictionary<Type, NetFsmState<Server>>()
            {
                { typeof(StartState), new StartState() },
                { typeof(ListenState), new ListenState() },
                { typeof(CloseState), new CloseState() },
                { typeof(DisposeState), new DisposeState() }
            });
            X.SystemLog.Debug($"[Server] new server create {_host}, data to thread {_fiber.ThreadId}");
        }

        public void Start()
        {
            if (_start) return;
            _start = true;
            _fsm.Start<ListenState>();
        }

        public void Close()
        {
            if (_disposed) return;
            _disposed = true;
            _fsm.Current.ChangeState<CloseState>().Forget();
        }

        public void RemoveConnection(IConnection connection)
        {
            if (_connections.ContainsKey(connection.Id))
                _connections.Remove(connection.Id);
        }

        public void AddConnection(Connection connection)
        {
            _connections.Add(connection.Id, connection);
            _onConnectionListChange?.Invoke(connection);
        }

        public void TriggerState(int newState)
        {
            _state.Value = (ServerState)newState;
        }
    }
}
