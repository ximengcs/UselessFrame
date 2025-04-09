
using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime.Extensions
{
    public static class Extensions
    {
        public static UniTask Start(this IFrameCore core)
        {
            return FrameManager.Start(core);
        }

        public static UniTask Destroy(this IFrameCore core)
        {
            return FrameManager.Destroy(core);
        }
    }
}
