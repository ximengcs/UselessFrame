
using UselessFrame.NewRuntime.World;

namespace UselessFrame.NewRuntime
{
    public static class X
    {
        private static ITypeManager _typeManager;
        private static IWorldManager _worldManager;

        public static ITypeManager Type => _typeManager;

        public static IWorldManager World => _worldManager;

        public static IRouter Router { get; }

        public static void Initialize(XSetting setting)
        {
            _typeManager = new TypeManager(setting.TypeFilter);
            _worldManager = new WorldManager();
        }
    }
}
