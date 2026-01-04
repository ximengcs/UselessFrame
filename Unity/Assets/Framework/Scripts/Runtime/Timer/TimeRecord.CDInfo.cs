
using UselessFrame.Runtime.Timer;

namespace UselessFrame.NewRuntime.Timers
{
    internal partial class TimeRecord
    {
        public class CDInfo
        {
            private ICanGetTime m_Updater;
            public float EndTime;
            public float CD;

            public CDInfo(ICanGetTime updater)
            {
                m_Updater = updater;
            }

            public float Suplus
            {
                get { return EndTime - m_Updater.Time; }
            }

            public bool Due
            {
                get { return m_Updater.Time >= EndTime; }
            }

            public void Reset()
            {
                EndTime = m_Updater.Time + CD;
            }
        }
    }
}
