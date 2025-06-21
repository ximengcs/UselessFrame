using System.Collections.Generic;
using System.Threading;
using static UselessFrame.NewRuntime.Fiber.Fiber;

namespace UselessFrame.NewRuntime.Fiber
{
    internal class MainFiber : IFiber
    {
        private int _threadId;
        private long _frame;
        private float _deltaTime;
        private CancellationTokenSource _disposeTokenSource;
        private FiberSynchronizationContext _context;
        private List<LoopItemInfo> _loopItems;

        public int ThreadId => _threadId;

        public long FrameCount => _frame;

        public float DeltaTime => _deltaTime;

        public bool IsMain => true;

        public MainFiber()
        {
            _loopItems = new List<LoopItemInfo>(1024);
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _context = new FiberSynchronizationContext(this, _threadId);
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

        public void Add(IFiberLoopItem loopItem)
        {
            _loopItems.Add(new LoopItemInfo(loopItem));
        }

        public void Update(float deltaTime)
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            _deltaTime = deltaTime;
            RunLoopItem();
            _context.OnUpdate(deltaTime);
            _frame++;
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
