
using System;
using UselessFrame.Net;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.World;
using System.Text;
using UselessFrame.NewRuntime.Net;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {
        private static MainFiber _mainFiber;
        private static ITypeManager _typeManager;
        private static IWorldManager _worldManager;
        private static ILogManager _logManager;
        private static IFiberManager _fiberManager;
        private static Dictionary<Guid, IServer> _servers;
        private static Dictionary<Guid, IConnection> _connections;

        public static ITypeManager Type => _typeManager;

        public static IWorldManager World => _worldManager;

        public static ILogManager SystemLog => _logManager;

        public static IFiberManager FiberManager => _fiberManager;

        public static IFiber MainFiber => _mainFiber;

        public static void Initialize(XSetting setting)
        {
            _typeManager = new TypeManager(setting.TypeFilter);
            _fiberManager = new FiberManager();
            _worldManager = new WorldManager();
            _logManager = new LogManager();
            _mainFiber = new MainFiber();
            _servers = new Dictionary<Guid, IServer>();
            _connections = new Dictionary<Guid, IConnection>();
            NetPoolUtility.InitializePool();
        }

        public static void Update(float deltaTime)
        {
            _mainFiber.Update(deltaTime);
        }

        public static void Shutdown()
        {
            List<IConnection> connections = new List<IConnection>(_connections.Values);
            foreach (IConnection connection in connections)
                connection.Close();
            List<IServer> servers = new List<IServer>(_servers.Values);
            foreach (IServer server in servers)
                server.Close();
            _connections.Clear();
            _servers.Clear();
        }

        internal static void RegisterServer(IServer server)
        {
            _servers.Add(server.Id, server);
        }

        internal static void UnRegisterServer(IServer server)
        {
            _servers.Remove(server.Id);
        }

        internal static void RegisterConnection(IConnection connection)
        {
            _connections.Add(connection.Id, connection);
        }

        internal static void UnRegisterConnection(IConnection connection)
        {
            _connections.Remove(connection.Id);
        }
    }
}
