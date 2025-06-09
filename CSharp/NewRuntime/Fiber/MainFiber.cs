
using System;
using XFrame.Core;
using System.Threading;
using System.Collections.Concurrent;

namespace UselessFrame.NewRuntime.Fiber
{
    internal class MainFiber : IFiber
    {
        private int _execTime;
        private ConcurrentQueue<Pair<SendOrPostCallback, object>> _actQueue;

        public MainFiber(int defaultTime = 64)
        {
            _execTime = defaultTime;
            _actQueue = new ConcurrentQueue<Pair<SendOrPostCallback, object>>();
        }

        public void Update()
        {
            long timeout = 0;
            long now = DateTime.Now.Ticks;
            while (_actQueue.Count > 0)
            {
                if (_actQueue.TryDequeue(out Pair<SendOrPostCallback, object> item))
                {
                    item.Key(item.Value);
                }
                long escape = DateTime.Now.Ticks - now;
                timeout += escape / TimeSpan.TicksPerMillisecond;
                if (_execTime != -1 && timeout >= _execTime)
                    break;
            }
        }

        public void Post(SendOrPostCallback d, object state)
        {
            _actQueue.Enqueue(Pair.Create(d, state));
        }
    }
}
