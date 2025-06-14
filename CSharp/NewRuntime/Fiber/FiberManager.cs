using System.Collections.Concurrent;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class FiberManager : IFiberManager
    {
        private ConcurrentDictionary<int, Fiber> _fibers;

        public FiberManager()
        {
            _fibers = new ConcurrentDictionary<int, Fiber>();
        }

        public IFiber Create()
        {
            Fiber fiber = new Fiber(this);
            _fibers.TryAdd(fiber.GetHashCode(), fiber);
            return fiber;
        }

        internal void Remove(Fiber fiber)
        {
            _fibers.TryRemove(fiber.GetHashCode(), out _);
        }
    }
}
