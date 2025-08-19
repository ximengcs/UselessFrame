
using Cysharp.Threading.Tasks;
using IdGen;
using System.Collections.Generic;
using System.Net;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Net;

namespace UselessFrame.Net
{
    internal class NetManager : IManagerInitializer, INetManager, IManagerDisposable
    {
        private IdGenerator _idGenerator;
        private Dictionary<long, IServer> _servers;
        private Dictionary<long, IConnection> _connections;

        public async UniTask Initialize(XSetting setting)
        {
            NetPoolUtility.InitializePool();
            IdGeneratorOptions opt = new IdGeneratorOptions(timeSource: X.GlobalTimeSource);
            _idGenerator = new IdGenerator(0, opt);
            _servers = new Dictionary<long, IServer>();
            _connections = new Dictionary<long, IConnection>();
        }

        public IServer GetServer(long serverId)
        {
            if (_servers.TryGetValue(serverId, out IServer server))
                return server;
            return default;
        }

        public IConnection GetConnection(long connectId)
        {
            if (_connections.TryGetValue(connectId, out IConnection connect))
                return connect;
            return default;
        }

        public void Dispose()
        {
            X.Log.Debug(FrameLogType.System, $"start dispose manager -> {GetType().Name}");
            List<IConnection> connections = new List<IConnection>(_connections.Values);
            foreach (Connection connection in connections)
            {
                X.Log.Debug(FrameLogType.System, $"start close connection -> {connection.Id}");
                connection.ForceClose();
            }
            List<IServer> servers = new List<IServer>(_servers.Values);
            foreach (Server server in servers)
            {
                X.Log.Debug(FrameLogType.System, $"start close server -> {server.Id}");
                server.Close();
            }
            _connections.Clear();
            _servers.Clear();
            X.Log.Debug(FrameLogType.System, $"dispose manager complete -> {GetType().Name}");
        }

        public IServer Create(int port, IFiber fiber)
        {
            Server server = new Server(_idGenerator.CreateId(), port, fiber);
            _servers.Add(server.Id, server);
            server.OnDestroy += RemoveServer;
            return server;
        }

        private void RemoveServer(IServer server)
        {
            _servers.Remove(server.Id);
        }

        public IConnection Connect(IPEndPoint ip, IFiber fiber)
        {
            Connection connection = new Connection(ip, fiber);
            _connections.Add(connection.Id, connection);
            connection.OnDestroy += RemoveConnect;
            return connection;
        }

        private void RemoveConnect(IConnection connection)
        {
            _connections.Remove(connection.Id);
        }

        public IConnection Connect(int port, IFiber fiber)
        {
            IPEndPoint ip = new IPEndPoint(NetUtility.GetLocalIPAddress(), port);
            return Connect(ip, fiber);
        }
    }
}
