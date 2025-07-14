
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {
        private static MainFiber _mainFiber;
        private static ITypeManager _typeManager;
        private static IWorldManager _worldManager;
        private static ILogManager _logManager;
        private static IFiberManager _fiberManager;
        private static CommandManager _commandManager;
        private static Dictionary<long, IServer> _servers;
        private static Dictionary<long, IConnection> _connections;

        public readonly static int Seed = (int)DateTime.UtcNow.Ticks;

        public static ITypeManager Type => _typeManager;

        public static IWorldManager World => _worldManager;

        public static ILogManager SystemLog => _logManager;

        public static IFiberManager FiberManager => _fiberManager;

        public static IFiber MainFiber => _mainFiber;

        public static ICommandManager Command => _commandManager;

        public static void Initialize(XSetting setting)
        {
            RandomUtility.Initialize(Seed);
            _logManager = new LogManager();
            AppDomain.CurrentDomain.UnhandledException += PrintSystemException;
            TaskScheduler.UnobservedTaskException += PrintTaskException;
            UniTaskScheduler.UnobservedTaskException += PrintUniTaskException;

            _typeManager = new TypeManager(setting.TypeFilter);
            _fiberManager = new FiberManager();
            _worldManager = new WorldManager();
            _mainFiber = new MainFiber();
            _servers = new Dictionary<long, IServer>();
            _connections = new Dictionary<long, IConnection>();
            _commandManager = new CommandManager();
            NetPoolUtility.InitializePool();
        }

        public static void Update(float deltaTime)
        {
            _mainFiber.Update(deltaTime);
        }

        public static void Shutdown()
        {
            AppDomain.CurrentDomain.UnhandledException -= PrintSystemException;
            TaskScheduler.UnobservedTaskException -= PrintTaskException;
            UniTaskScheduler.UnobservedTaskException -= PrintUniTaskException;
            List<IConnection> connections = new List<IConnection>(_connections.Values);
            foreach (IConnection connection in connections)
                connection.Close();
            List<IServer> servers = new List<IServer>(_servers.Values);
            foreach (IServer server in servers)
                server.Close();
            _connections.Clear();
            _servers.Clear();
        }

        public static IServer GetServer(long id)
        {
            if (_servers.TryGetValue(id, out IServer server))
                return server;
            return null;
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
