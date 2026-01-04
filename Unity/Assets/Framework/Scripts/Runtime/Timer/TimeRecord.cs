
using System.Collections.Generic;
using UselessFrame.Runtime.Timer;

namespace UselessFrame.NewRuntime.Timers
{
    internal partial class TimeRecord : ITimeRecord
    {
        private ICanGetTime m_Updater;
        private Dictionary<int, CDInfo> m_Times;

        public TimeRecord()
        {
            m_Times = new Dictionary<int, CDInfo>();
        }

        /// <summary>
        /// 设置更新器
        /// </summary>
        /// <param name="updater"></param>
        public void SetUpdater(ICanGetTime updater)
        {
            m_Updater = updater;
        }

        /// <summary>
        /// 开始记录一个CD
        /// </summary>
        /// <param name="key">CD键(使用此键查看CD状态)</param>
        /// <param name="cd">cd时间</param>
        public void Record(int key, float cd)
        {
            CDInfo info = new CDInfo(m_Updater);
            info.CD = cd;
            info.EndTime = m_Updater.Time;
            m_Times[key] = info;
        }

        /// <summary>
        /// 记录默认键的CD
        /// </summary>
        /// <param name="cd">cd时间</param>
        public void Record(float cd)
        {
            Record(default, cd);
        }

        /// <summary>
        /// 重置一个cd, 调用后重置CD时间
        /// </summary>
        /// <param name="key">CD键</param>
        public void Reset(int key)
        {
            if (m_Times.TryGetValue(key, out CDInfo info))
                info.Reset();
        }

        /// <summary>
        /// 重置CD
        /// </summary>
        public void Reset()
        {
            Reset(default);
        }

        /// <summary>
        /// 检查一个CD的状态
        /// </summary>
        /// <param name="key">CD键</param>
        /// <param name="reset">如果检查到的状态为到期，是否重置CD时间</param>
        /// <returns>true表示到期，false表示未到CD时间</returns>
        public bool Check(int key, bool reset = false)
        {
            if (m_Times.TryGetValue(key, out CDInfo info))
            {
                if (info.Due)
                {
                    if (reset)
                        info.Reset();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查一个CD的状态
        /// </summary>
        /// <param name="reset">如果检查到的状态为到期，是否重置CD时间</param>
        /// <returns>true表示到期，false表示未到CD时间</returns>
        public bool Check(bool reset = false)
        {
            return Check(default, reset);
        }

        /// <summary>
        /// 检查CD的时间状态
        /// </summary>
        /// <param name="key">CD键</param>
        /// <returns>时间</returns>
        public float CheckTime(int key)
        {
            if (m_Times.TryGetValue(key, out CDInfo info))
            {
                return info.Suplus;
            }

            return -1;
        }

        /// <summary>
        /// 检查默认键的CD时间状态
        /// </summary>
        /// <returns>时间</returns>
        public float CheckTime()
        {
            return CheckTime(default);
        }
    }
}
