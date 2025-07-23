using IdGen;
using System;
using UselessFrame.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.ECS;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Randoms;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.NewRuntime.Net;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {
        private static TimeTicksSource  _timeSource;
        private static TimeRandom       _random;
        private static TypeManager      _typeManager;
        private static WorldManager     _worldManager;
        private static LogManager       _logManager;
        private static FiberManager     _fiberManager;
        private static CommandManager   _commandManager;
        private static NetManager       _netManager;
        private static PoolManager      _poolManager;
        private static ModuleCore        _moduleCore;

        public static IRandom Random => _random;

        public static ITimeSource GlobalTimeSource => _timeSource;

        public static ITypeManager Type => _typeManager;

        public static WorldManager World => _worldManager;

        public static ILogManager SystemLog => _logManager;

        public static IFiberManager FiberManager => _fiberManager;

        public static ICommandManager Command => _commandManager;

        public static INetManager Net => _netManager;

        public static IPoolManager Pool => _poolManager;

        public static IModuleCore Module => _moduleCore;

        public static void Initialize(XSetting setting)
        {
            AppDomain.CurrentDomain.UnhandledException += PrintSystemException;
            TaskScheduler.UnobservedTaskException += PrintTaskException;
            UniTaskScheduler.UnobservedTaskException += PrintUniTaskException;

            _timeSource = new TimeTicksSource();
            _random = new TimeRandom(_timeSource);

            _logManager     = new LogManager();
            _typeManager    = new TypeManager();
            _fiberManager   = new FiberManager();
            _netManager     = new NetManager();
            _worldManager   = new WorldManager();
            _commandManager = new CommandManager();
            _poolManager    = new PoolManager();
            _moduleCore     = new ModuleCore(default);

            InitManager(_logManager, setting);
            InitManager(_typeManager, setting);
            InitManager(_fiberManager, setting);
            InitManager(_netManager, setting);
            InitManager(_worldManager, setting);
            InitManager(_commandManager, setting);
            InitManager(_poolManager, setting);
        }

        public static void Update(float deltaTime)
        {
            _fiberManager.UpdateMain(deltaTime);
            _moduleCore.Trigger<IModuleUpdater>(deltaTime);
        }

        public static void Shutdown()
        {
            AppDomain.CurrentDomain.UnhandledException -= PrintSystemException;
            TaskScheduler.UnobservedTaskException -= PrintTaskException;
            UniTaskScheduler.UnobservedTaskException -= PrintUniTaskException;

            DisposeManager(_fiberManager);
            DisposeManager(_netManager);
        }

        private static void InitManager(object manager, XSetting setting)
        {
            if (manager is IManagerInitializer initializer)
                initializer.Initialize(setting);
        }

        private static void DisposeManager(object manager)
        {
            if (manager is IManagerDisposable initializer)
                initializer.Dispose();
        }
    }
}
