using System;
using System.Collections.Concurrent;
using System.Threading;
using XFrame.Core;

namespace XFrame.Modules.Threads
{
    /// <summary>
    /// 主线程上下文处理
    /// </summary>
    public class MainSynchronizationContext : SynchronizationContext
    {
        #region Inner Fields

        private int m_MainThread;
        private ConcurrentQueue<Pair<SendOrPostCallback, object>> m_ActQueue;
        private const long DEFAULT_TIMEOUT = 10;

        #endregion

        #region Interface

        /// <summary>
        /// 最大超时(毫秒)
        /// </summary>
        public long ExecTimeout { get; set; }

        #endregion

        #region IModule Life Fun

        public MainSynchronizationContext()
        {
            m_MainThread = Thread.CurrentThread.ManagedThreadId;
            m_ActQueue = new ConcurrentQueue<Pair<SendOrPostCallback, object>>();
            ExecTimeout = DEFAULT_TIMEOUT;
            SetSynchronizationContext(this);
        }

        public void OnUpdate(float escapeTime)
        {
            if (m_ActQueue.Count <= 0)
                return;

            if (m_MainThread == Thread.CurrentThread.ManagedThreadId)
            {
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
        }

        #endregion

        #region System Life Fun

        /// <inheritdoc/>
        public override void Post(SendOrPostCallback d, object state)
        {
            m_ActQueue.Enqueue(Pair.Create(d, state));
        }
        #endregion
    }
}