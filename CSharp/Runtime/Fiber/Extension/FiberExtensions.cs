
using Cysharp.Threading.Tasks;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    internal static class FiberExtensions
    {
        public static IFiber GetCurrentContextFiber()
        {
            Fiber.FiberSynchronizationContext context = (Fiber.FiberSynchronizationContext)SynchronizationContext.Current;
            return context.Fiber;
        }
    }
}
