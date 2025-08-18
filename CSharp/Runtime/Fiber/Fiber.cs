
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class Fiber : IFiber
    {
        private Thread _thread;
        private FiberManager _fiberManager;
        private FiberSynchronizationContext _context;
        private CancellationTokenSource _disposeTokenSource;
        private UniTaskCompletionSource _runAllTask;
        private long _frame;
        private float _deltaTime;
        private float _time;
        private List<LoopItemInfo> _loopItems;

        public SynchronizationContext Context => _context;

        public long FrameCount => _frame;

        public float DeltaTime => _deltaTime;

        public int ThreadId => _thread.ManagedThreadId;

        public bool IsMain => false;

        public float Time => _time;

        public int ExecuteCount => _loopItems.Count + _context.Count;

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

        public SwitchToSynchronizationContextAwaitable Switch()
        {
            return UniTask.SwitchToSynchronizationContext(_context, _disposeTokenSource.Token);
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

        public UniTask RunAll()
        {
            X.Log.Debug($"Check Run All {ExecuteCount}");
            _runAllTask = new UniTaskCompletionSource();
            return _runAllTask.Task;
        }

        private void Run()
        {
            if (_disposeTokenSource.IsCancellationRequested)
                return;

            SynchronizationContext.SetSynchronizationContext(_context);
            FiberUtility.RunLoopSleep1(RunUpdate, _disposeTokenSource.Token);
        }

        private void RunUpdate(float deltaTime)
        {
            _deltaTime = deltaTime;
            _time += deltaTime;
            RunLoopItem();
            _context.OnUpdate(_deltaTime);
            _frame++;

            if (_runAllTask != null && ExecuteCount <= 0)
            {
                _runAllTask.TrySetResult();
                _runAllTask = null;
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
