
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Timer;

namespace UselessFrame.NewRuntime.Timers
{
    internal partial class TimeRecord : IPoolObject
    {
        public int PoolKey => 0;

        IPool IPoolObject.InPool { get; set; }

        void IPoolObject.OnCreate()
        {
            m_Times = new Dictionary<int, CDInfo>();
        }

        void IPoolObject.OnDelete()
        {
        }

        void IPoolObject.OnRelease()
        {
        }

        void IPoolObject.OnRequest(object userData)
        {
            if (userData != null && userData is ICanGetTime time)
            {
                m_Updater = time;
            }
            else
            {
                m_Updater = X.Fiber.MainFiber;
            }
        }
    }
}
