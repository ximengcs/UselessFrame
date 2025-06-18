using System.Threading;
using System.Diagnostics;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class Fiber : IFiber
    {
        private Thread _thread;
        private FiberManager _fiberManager;
        private FiberSynchronizationContext _context;
        private CancellationTokenSource _disposeTokenSource;
        private long _frame;

        public int ThreadId => _thread.ManagedThreadId;

        public Fiber(FiberManager fiberManager)
        {
            _fiberManager = fiberManager;
            _context = new FiberSynchronizationContext(this);
            _disposeTokenSource = new CancellationTokenSource();
            _thread = new Thread(Run)
            {
                IsBackground = true,
            };
            _thread.Start();
        }

        public void Dispose()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _disposeTokenSource.Cancel();
            _fiberManager.Remove(this);
            _thread = null;
        }

        public void Post(SendOrPostCallback d, object state)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _context.Post(d, state);
        }

        private void Run()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            SynchronizationContext.SetSynchronizationContext(_context);
            Stopwatch sw = Stopwatch.StartNew();
            while (!_disposeTokenSource.IsCancellationRequested)
            {
                sw.Stop();
                _context.OnUpdate(sw.ElapsedMilliseconds / 1000d);
                sw.Restart();
                Thread.Sleep(1);
            }
        }
    }
}
