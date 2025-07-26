using Cysharp.Threading.Tasks;
using IdGen;
using System;
using System.Threading.Tasks;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Commands;
using UselessFrame.NewRuntime.Cryptos;
using UselessFrame.NewRuntime.ECS;
using UselessFrame.NewRuntime.Events;
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.Net;
using UselessFrame.NewRuntime.Randoms;
using UselessFrame.NewRuntime.StateMachine;
using UselessFrame.NewRuntime.Utilities;
using UselessFrame.Runtime;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Types;
using XFrame.Modules.Archives;
using XFrame.Modules.Procedure;

namespace UselessFrame.NewRuntime
{
    public static partial class X
    {
        private static TimeTicksSource _timeSource;
        private static TimeRandom _random;
        private static TypeManager _typeManager;
        private static WorldManager _worldManager;
        private static LogManager _logManager;
        private static FiberManager _fiberManager;
        private static CommandManager _commandManager;
        private static NetManager _netManager;
        private static PoolManager _poolManager;
        private static EventManager _eventManager;
        private static FsmManager _fsmManager;
        private static CryptoManager _cryptoManager;
        private static ArchiveModule _archiveModule;
        private static ProcedureModule _procedure;
        private static ModuleCore _moduleCore;
        private static XSetting _setting;
        private static bool _initialized;

        public static IRandom Random => _random;

        public static ITimeSource GlobalTimeSource => _timeSource;

        public static ITypeManager Type => _typeManager;

        public static WorldManager World => _worldManager;

        public static ILogManager Log => _logManager;

        public static IFiberManager Fiber => _fiberManager;

        public static ICommandManager Command => _commandManager;

        public static INetManager Net => _netManager;

        public static IPoolManager Pool => _poolManager;

        public static IEventManager Event => _eventManager;

        public static ICryptoManager Crypto => _cryptoManager;

        public static IFsmManager Fsm => _fsmManager;

        public static ArchiveModule Archive => _archiveModule;

        public static IModuleCore Module => _moduleCore;

        public static async UniTask Initialize(XSetting setting)
        {
            _setting = setting;
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
            _eventManager   = new EventManager();
            _fsmManager     = new FsmManager();
            _cryptoManager  = new CryptoManager();
            _archiveModule  = new ArchiveModule();
            _procedure      = new ProcedureModule();

            InitManager(_logManager);
            InitManager(_typeManager);
            InitManager(_fiberManager);
            InitManager(_netManager);
            InitManager(_worldManager);
            InitManager(_commandManager);
            InitManager(_poolManager);
            InitManager(_eventManager);
            InitManager(_fsmManager);
            InitManager(_cryptoManager);
            InitManager(_archiveModule);
            InitManager(_procedure);
            _moduleCore = new ModuleCore(default);
            InitLogger();
            InitModules();
            await _moduleCore.Start();
            _initialized = true;
            _procedure.Start();
        }

        public static void Update(float deltaTime)
        {
            if (!_initialized) return;
            _eventManager.OnUpdate(deltaTime);
            _fiberManager.UpdateMain(deltaTime);
            _fsmManager.OnUpdate(deltaTime);
            _archiveModule.OnUpdate(deltaTime);
            _moduleCore.Trigger<IModuleUpdater>(deltaTime);
        }

        public static void Shutdown()
        {
            AppDomain.CurrentDomain.UnhandledException -= PrintSystemException;
            TaskScheduler.UnobservedTaskException -= PrintTaskException;
            UniTaskScheduler.UnobservedTaskException -= PrintUniTaskException;

            _ = _moduleCore.Destroy();
            DisposeManager(_fiberManager);
            DisposeManager(_netManager);
            DisposeManager(_fsmManager);
            DisposeManager(_archiveModule);
        }

        private static void InitLogger()
        {
            if (_setting.Loggers == null)
                return;

            foreach (Type type in _setting.Loggers)
                _logManager.AddLogger(type);
        }

        private static void InitModules()
        {
            if (_setting.ModuleAttributes == null)
                return;

            for (int i = 0; i < _setting.ModuleAttributes.Length; i++)
            {
                Type attrType = _setting.ModuleAttributes[i];
                object param = _setting.ModuleParams != null ? _setting.ModuleParams[i] : null;
                ITypeCollection collection = _typeManager.GetOrNewWithAttr(attrType);
                foreach (Type type in collection)
                    _moduleCore.AddModule(type, param);
            }
        }

        private static void InitManager(object manager)
        {
            if (manager is IManagerInitializer initializer)
                initializer.Initialize(_setting);
        }

        private static void DisposeManager(object manager)
        {
            if (manager is IManagerDisposable initializer)
                initializer.Dispose();
        }
    }
}
