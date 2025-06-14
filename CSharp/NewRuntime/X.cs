
using UselessFrame.NewRuntime.Fiber;
using UselessFrame.NewRuntime.World;

namespace UselessFrame.NewRuntime
{
    public static class X
    {
        private static ITypeManager _typeManager;
        private static IWorldManager _worldManager;
        private static ILogManager _logManager;
        private static IFiberManager _fiberManager;
        private static MainFiber _mainFiber;

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
        }

        public static void Update(float deltaTime)
        {
            _mainFiber.Update(deltaTime);
        }
    }
}
