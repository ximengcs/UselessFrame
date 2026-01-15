
using UselessFrame.NewRuntime;
using UselessFrame.ResourceManager;
using UselessFrame.UIElements;
using UselessFrameUnity.RedPointSystem;

namespace UselessFrameUnity
{
    public static class G
    {
        private static UIModule _ui;

        public static UIModule UI => _ui ??= X.Module.Get<UIModule>();

        private static IResourceModule _localRes;

        public static IResourceModule LocalRes => _localRes ??= X.Module.Get<IResourceModule>();

        private static IRedPointModule _redPoint;

        public static IRedPointModule RedPoint => _redPoint ??= (IRedPointModule)X.Module.Get(typeof(RedPointModule));
    }
}
