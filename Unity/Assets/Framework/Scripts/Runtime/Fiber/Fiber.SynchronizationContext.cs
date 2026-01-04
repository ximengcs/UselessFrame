
using System;
using System.Collections.Concurrent;
using System.Threading;
using UselessFrame.Runtime;
using XFrame.Core;

namespace UselessFrame.NewRuntime.Fiber
{
    public partial class Fiber
    {
        internal class FiberSynchronizationContext : SynchronizationContext, IUpdater
        {
            #region Inner Fields
            private int _threadId;
            private IFiber _fiber;
            private bool _disposed;
            private ConcurrentQueue<Pair<SendOrPostCallback, object>> m_ActQueue;
            private const long DEFAULT_TIMEOUT = -1;
            #endregion

            #region Interface
            /// <summary>
            /// 最大超时(毫秒)
            /// </summary>
            public long ExecTimeout { get; set; }

            public IFiber Fiber => _fiber;

            public int Count => m_ActQueue.Count;
            #endregion

            #region IModule Life Fun
            public FiberSynchronizationContext(IFiber fiber, int checkThreadId = -1)
            {
                _fiber = fiber;
                _threadId = checkThreadId;
                m_ActQueue = new ConcurrentQueue<Pair<SendOrPostCallback, object>>();
                ExecTimeout = DEFAULT_TIMEOUT;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                m_ActQueue.Clear();
                m_ActQueue = null;
                ExecTimeout = default;
            }

            public void OnUpdate(float escapeTime)
            {
                if (_disposed) return;

                if (_threadId != -1 && _threadId != Thread.CurrentThread.ManagedThreadId)
                    return;

                if (m_ActQueue.Count <= 0)
                    return;

                long timeout = 0;
                long now = DateTime.Now.Ticks;
                while (m_ActQueue.Count > 0)
                {
                    if (m_ActQueue.TryDequeue(out Pair<SendOrPostCallback, object> item))
                    {
                        item.Key(item.Value);
                    }
                    long escape = DateTime.Now.Ticks - now;
                    timeout += escape / TimeSpan.TicksPerMillisecond;
                    if (ExecTimeout != -1 && timeout >= ExecTimeout)
                        break;
                }
            }
            #endregion

            #region System Life Fun

            /// <inheritdoc/>
            public override void Post(SendOrPostCallback d, object state)
            {
                m_ActQueue.Enqueue(Pair.Create(d, state));
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                d(state);
            }
            #endregion
        }
    }
}
