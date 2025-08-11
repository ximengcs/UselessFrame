using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class FiberManager : IFiberManager, IManagerDisposable, IManagerUpdater
    {
        private static MainFiber _mainFiber;
        private ConcurrentDictionary<int, Fiber> _fibers;
        private ConcurrentDictionary<SynchronizationContext, IFiber> _contextMap;

        public IFiber MainFiber => _mainFiber;

        public FiberManager()
        {
            _mainFiber = new MainFiber();
            _fibers = new ConcurrentDictionary<int, Fiber>();
            _contextMap = new ConcurrentDictionary<SynchronizationContext, IFiber>();
            _contextMap.TryAdd(_mainFiber.Context, _mainFiber);
        }

        public IFiber GetFiber(SynchronizationContext context)
        {
            if (_contextMap.TryGetValue(context, out var fiber))
                return fiber;
            return null;
        }

        public IFiber Create()
        {
            Fiber fiber = new Fiber(this);
            _fibers.TryAdd(fiber.GetHashCode(), fiber);
            _contextMap.TryAdd(fiber.Context, fiber);
            return fiber;
        }

        public void Update(float deltaTime)
        {
            _mainFiber.Update(deltaTime);
        }

        public async UniTask Dispose()
        {
            InnerDispose();
            await UniTask.CompletedTask;
        }

        private void InnerDispose()
        {
            foreach (var fiberEntry in _fibers)
            {
                fiberEntry.Value.Dispose();
            }
            _fibers = null;
            _contextMap = null;
        }

        internal void Remove(Fiber fiber)
        {
            _contextMap.TryRemove(fiber.Context, out _);
            _fibers.TryRemove(fiber.GetHashCode(), out _);
        }
    }
}
