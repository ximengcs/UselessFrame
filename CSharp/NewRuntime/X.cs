
using UselessFrame.NewRuntime.Router;
using UselessFrame.NewRuntime.World;

namespace UselessFrame.NewRuntime
{
    public static class X
    {
        private static ITypeManager _typeManager;
        private static IWorldManager _worldManager;
        private static ILogManager _logManager;

        public static ITypeManager Type => _typeManager;

        public static IWorldManager World => _worldManager;

        public static ILogManager SystemLog => _logManager;

        public static void Initialize(XSetting setting)
        {
            _typeManager = new TypeManager(setting.TypeFilter);
            _worldManager = new WorldManager();
            _logManager = new LogManager();
        }
    }
}
