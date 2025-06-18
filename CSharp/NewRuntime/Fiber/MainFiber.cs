using System.Threading;
using static UselessFrame.NewRuntime.Fiber.Fiber;

namespace UselessFrame.NewRuntime.Fiber
{
    internal class MainFiber : IFiber
    {
        private int _threadId;
        private CancellationTokenSource _disposeTokenSource;
        private FiberSynchronizationContext _context;

        public int ThreadId => _threadId;

        public MainFiber()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _context = new FiberSynchronizationContext(_threadId);
            _disposeTokenSource = new CancellationTokenSource();
            SynchronizationContext.SetSynchronizationContext(_context);
        }

        public void Dispose()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _disposeTokenSource.Cancel();
        }

        public void Post(SendOrPostCallback d, object state)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _context.Post(d, state);
        }

        public void Update(float deltaTime)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _context.OnUpdate(deltaTime);
        }
    }
}
