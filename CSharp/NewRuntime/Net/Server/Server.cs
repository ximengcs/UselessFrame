
using System;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    internal partial class Server : IServer, INetStateTrigger
    {
        private bool _disposed;
        private bool _start;
        private IPEndPoint _host;
        private TcpListener _listener;
        private NetFsm<Server> _fsm;
        private Dictionary<Guid, Connection> _connections;
        private Action<ServerState, ServerState> _onStateChange;
        private Action<IConnection> _onConnectionListChange;

        public event Action<ServerState, ServerState> StateChangeEvent
        {
            add { _onStateChange += value; }
            remove { _onStateChange -= value; }
        }

        public event Action<IConnection> NewConnectionEvent
        {
            add { _onConnectionListChange += value; }
            remove { _onConnectionListChange -= value; }
        }

        public IReadOnlyList<IConnection> Connections => new List<IConnection>(_connections.Values);

        public Server(int port)
        {
            _disposed = false;
            _connections = new Dictionary<Guid, Connection>();
            _host = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            _listener = new TcpListener(_host);
            _fsm = new NetFsm<Server>(this, null);
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

        public void TriggerState(int oldState, int newState)
        {
            _onStateChange?.Invoke((ServerState)oldState, (ServerState)newState);
        }
    }
}
