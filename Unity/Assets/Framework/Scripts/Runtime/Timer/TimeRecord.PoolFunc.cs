
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Timer;

namespace UselessFrame.NewRuntime.Timers
{
    internal partial class TimeRecord : IPoolObject
    {
        public int PoolKey => 0;

        IPool IPoolObject.InPool { get; set; }

        void IPoolObject.OnCreate(object userData)
        {
            m_Times = new Dictionary<int, CDInfo>();
            if (userData != null && userData is ICanGetTime time)
            {
                m_Updater = time;
            }
            else
            {
                m_Updater = X.Fiber.MainFiber;
            }
        }

        void IPoolObject.OnDelete()
        {
        }

        void IPoolObject.OnRelease()
        {
        }

        void IPoolObject.OnRequest()
        {
            
        }
    }
}
