using XFrame.Modules.Event;
using XFrame.Modules.Pools;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件组时间触发, 此条件仅会影响目标条件组句柄中的条件，
    /// 但当触发的条件是全局条件时，则会触发全局<see cref="IConditionCompare{T}.OnEventTrigger"/>
    /// <para>
    /// 当条件为全局条件时，首先会触发<see cref="IConditionCompare{T}.OnEventTrigger"/>，接着触发条件组事件,
    /// 当条件为组内条件时，仅仅发某个条件组中的事件，不对其他组产生影响
    /// </para>
    /// </summary>
    public class ConditionGroupEvent : XEvent
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
                    s_EventId = typeof(ConditionGroupEvent).GetHashCode();
                return s_EventId;
            }
        }

        /// <summary>
        /// 触发的目标条件组句柄
        /// </summary>
        public IConditionGroupHandle Handle { get; private set; }

        /// <summary>
        /// 触发目标条件类型
        /// </summary>
        public int Target { get; private set; }

        /// <summary>
        /// 触发目标条件参数
        /// <para>
        /// 可为数量等, 定义为<see cref="IConditionCompare{T}.OnEventTrigger"/>和
        /// <see cref="IConditionCompare{T}.Check"/>的接受参数, 由<see cref="IConditionCompare"/>的具体实现类解析判断。
        /// </para>
        /// </summary>
        public object Param { get; private set; }

        /// <summary>
        /// 创建事件实例(从对象池中创建)
        /// </summary>
        /// <param name="handle">目标条件组句柄</param>
        /// <param name="target">目标条件类型</param>
        /// <param name="param">目标条件参数</param>
        /// <returns>事件实例</returns>
        public static ConditionGroupEvent Create(IConditionGroupHandle handle, int target, object param)
        {
            ConditionGroupEvent e = References.Require<ConditionGroupEvent>();
            e.Handle = handle;
            e.Target = target;
            e.Param = param;
            e.Id = EventId;
            return e;
        }

        /// <inheritdoc/>
        protected internal override void OnReleaseFromPool()
        {
            base.OnReleaseFromPool();
            Handle = null;
            Target = 0;
            Param = null;
        }
    }
}
