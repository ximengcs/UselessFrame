
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Events;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件事件
    /// <para>
    /// 此事件仅会影响目标句柄<see cref="Handle"/>的指定条件，
    /// 但当触发的条件是全局条件时，则会触发全局<see cref="IConditionCompare{T}.OnEventTrigger"/>
    /// </para>
    /// </summary>
    public class SpecificConditionEvent : XEvent
    {
        private static int s_EventId;

        /// <summary>
        /// 事件Id
        /// </summary>
        public static int EventId
        {
            get
            {
                if (s_EventId == 0)
                    s_EventId = typeof(SpecificConditionEvent).GetHashCode();
                return s_EventId;
            }
        }

        /// <summary>
        /// 目标条件句柄
        /// </summary>
        public IConditionHandle Handle { get; private set; }

        /// <summary>
        /// 触发参数
        /// </summary>
        public object Param { get; private set; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="handle">条件句柄</param>
        /// <param name="param">触发参数</param>
        /// <returns>事件实例</returns>
        public static SpecificConditionEvent Create(IConditionHandle handle, object param)
        {
            SpecificConditionEvent e = X.Pool.Require<SpecificConditionEvent>();
            e.Handle = handle;
            e.Param = param;
            e.Id = EventId;
            return e;
        }

        /// <inheritdoc/>
        protected internal override void OnReleaseFromPool()
        {
            base.OnReleaseFromPool();
            Handle = null;
            Param = null;
        }
    }
}
