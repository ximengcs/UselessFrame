
using UselessFrame.NewRuntime;
using UselessFrame.UIElements;

namespace UselessFrameUnity
{
    public static class G
    {
        private static UIModule _ui;

        public static UIModule UI => _ui ??= X.Module.Get<UIModule>();
    }
}
