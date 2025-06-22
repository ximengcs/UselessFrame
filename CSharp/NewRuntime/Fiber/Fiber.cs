
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class Fiber : IFiber
    {
        private Thread _thread;
        private FiberManager _fiberManager;
        private FiberSynchronizationContext _context;
        private CancellationTokenSource _disposeTokenSource;
        private long _frame;
        private float _deltaTime;
        private List<LoopItemInfo> _loopItems;

        public long FrameCount => _frame;

        public float DeltaTime => _deltaTime;

        public int ThreadId => _thread.ManagedThreadId;

        public bool IsMain => false;

        public Fiber(FiberManager fiberManager)
        {
            _loopItems = new List<LoopItemInfo>(1024);
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

        public void Add(IFiberLoopItem loopItem)
        {
            _loopItems.Add(new LoopItemInfo(loopItem));
        }

        private void Run()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            SynchronizationContext.SetSynchronizationContext(_context);
            Stopwatch sw = Stopwatch.StartNew();
            while (!_disposeTokenSource.IsCancellationRequested)
            {
                _deltaTime = sw.ElapsedMilliseconds / 1000f;
                RunLoopItem();
                _context.OnUpdate(_deltaTime);
                _frame++;
                sw.Stop();
                sw.Restart();
                Thread.Sleep(1);
            }
        }

        private void RunLoopItem()
        {
            for (int i = _loopItems.Count - 1; i >= 0; i--)
            {
                LoopItemInfo info = _loopItems[i];
                if (!info.Item.MoveNext())
                {
                    info.ShouldRemove = true;
                    _loopItems[i] = info;
                }
            }

            for (int i = _loopItems.Count - 1; i >= 0; i--)
            {
                LoopItemInfo info = _loopItems[i];
                if (info.ShouldRemove)
                {
                    int lastIndex = _loopItems.Count - 1;
                    if (lastIndex > 0)
                        _loopItems[i] = _loopItems[lastIndex];
                    _loopItems.RemoveAt(lastIndex);
                }
            }
        }
    }
}
