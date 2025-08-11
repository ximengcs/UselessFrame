
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    internal static class FiberExtensions
    {
        public static IFiber GetCurrentContextFiber()
        {
            FiberManager manager = (FiberManager)X.Fiber;
            return manager.GetFiber(SynchronizationContext.Current);
        }
    }
}
