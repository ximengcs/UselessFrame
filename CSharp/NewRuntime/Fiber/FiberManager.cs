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

        public void Dispose()
        {
            InnerDispose();
        }

        private void InnerDispose()
        {
            foreach (var fiberEntry in _fibers)
            {
                fiberEntry.Value.Dispose();
            }
            _fibers = null;
        }

        internal void Remove(Fiber fiber)
        {
            _fibers.TryRemove(fiber.GetHashCode(), out _);
        }
    }
}
