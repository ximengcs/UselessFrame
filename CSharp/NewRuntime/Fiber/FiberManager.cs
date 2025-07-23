using System.Collections.Concurrent;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class FiberManager : IFiberManager, IManagerDisposable
    {
        private static MainFiber _mainFiber;
        private ConcurrentDictionary<int, Fiber> _fibers;

        public IFiber MainFiber => _mainFiber;

        public FiberManager()
        {
            _mainFiber = new MainFiber();
            _fibers = new ConcurrentDictionary<int, Fiber>();
        }

        public IFiber Create()
        {
            Fiber fiber = new Fiber(this);
            _fibers.TryAdd(fiber.GetHashCode(), fiber);
            return fiber;
        }

        public void UpdateMain(float deltaTime)
        {
            _mainFiber.Update(deltaTime);
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
