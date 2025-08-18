using IdGen;
using System;
using UselessFrame.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
using XFrame.Modules.Conditions;
using System.Collections.Generic;

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
        private static ConditionModule _condition;
        private static ModuleCore _moduleCore;
        private static XSetting _setting;
        private static bool _initialized;

        private static List<IManagerUpdater> _managerUpdaters;
        private static List<IManagerDisposable> _managerDisposes;

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

        public static IConditionModule Condition => _condition;

        public static IModuleCore Module => _moduleCore;

        public static async UniTask Initialize(XSetting setting)
        {
            _setting = setting;
            _logManager = new LogManager(setting.Loggers);
            AppDomain.CurrentDomain.UnhandledException += PrintSystemException;
            TaskScheduler.UnobservedTaskException      += PrintTaskException;
            UniTaskScheduler.UnobservedTaskException   += PrintUniTaskException;

            _managerUpdaters = new List<IManagerUpdater>();
            _managerDisposes = new List<IManagerDisposable>();
            _timeSource      = new TimeTicksSource();
            _random          = new TimeRandom(_timeSource);

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
            _condition      = new ConditionModule();
            _procedure      = new ProcedureModule();
            _moduleCore     = new ModuleCore(default);

            await InitManager(_typeManager);
            await InitManager(_logManager);
            await InitManager(_fiberManager);
            await InitManager(_netManager);
            await InitManager(_worldManager);
            await InitManager(_commandManager);
            await InitManager(_poolManager);
            await InitManager(_eventManager);
            await InitManager(_fsmManager);
            await InitManager(_cryptoManager);
            await InitManager(_archiveModule);
            await InitManager(_condition);
            await InitManager(_procedure);
            await InitManager(_moduleCore);
            InitModules();

            await _moduleCore.Start();
            _initialized = true;
            _procedure.Start();
        }

        public static void Update(float deltaTime)
        {
            if (!_initialized) return;
            foreach (IManagerUpdater updater in _managerUpdaters)
                updater.Update(deltaTime);
        }

        public static async UniTask Shutdown()
        {
            AppDomain.CurrentDomain.UnhandledException -= PrintSystemException;
            TaskScheduler.UnobservedTaskException      -= PrintTaskException;
            UniTaskScheduler.UnobservedTaskException   -= PrintUniTaskException;

            for (int i = _managerDisposes.Count - 1; i >= 0; i--)
                await _managerDisposes[i].Dispose();
        }

        private static void InitModules()
        {
            if (_setting.ModuleAttributes != null)
            {
                for (int i = 0; i < _setting.ModuleAttributes.Length; i++)
                {
                    Type attrType = _setting.ModuleAttributes[i];
                    ITypeCollection collection = _typeManager.GetOrNewWithAttr(attrType);
                    foreach (Type type in collection)
                        _moduleCore.Add(type, null);
                }
            }

            if (_setting.Modules != null)
            {
                foreach (ValueTuple<Type, object> entry in _setting.Modules)
                {
                    _moduleCore.Add(entry.Item1, entry.Item2);
                }
            }
        }

        private static async UniTask InitManager(object manager)
        {
            if (manager is IManagerInitializer initializer)
                await initializer.Initialize(_setting);
            if (manager is IManagerUpdater updater)
                _managerUpdaters.Add(updater);
            if (manager is IManagerDisposable disposer)
                _managerDisposes.Add(disposer);
        }
    }
}
