using System;
using XFrame.Core;

namespace XFrame.Modules.Conditions
{
    /// <summary>
    /// 条件项句柄(单个条件)
    /// <para>
    /// 在初始化时会调用<see cref="IConditionCompare.CheckFinish"/>检查条件完成状态
    /// 当触发<see cref="ConditionEvent"/>事件时，会调用<see cref="IConditionCompare{T}.OnEventTrigger"/>,
    /// 接着调用所有满足条件项的句柄<see cref="IConditionCompare{T}.Check"/>检查是否完成，
    /// </para>
    /// </summary>
    public interface IConditionHandle : IDataProvider
    {
        /// <summary>
        /// 条件项辅助器<see cref="IConditionCompare"/>的实例Id，默认使用全局辅助器
        /// </summary>
        int InstanceId { get; }

        /// <summary>
        /// 条件目标
        /// <para>
        /// <see cref="ConditionEvent.Target"/> 触发的目标会根据此值匹配句柄实例
        /// </para>
        /// <para>
        /// <see cref="IConditionCompare.Target"/> 具体的实现类会匹配到此值
        /// </para>
        /// </summary>
        int Target { get; }

        /// <summary>
        /// 条件是否达成
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// 条件需要达成的目标参数，如数量等
        /// </summary>
        UniversalParser Param { get; }

        /// <summary>
        /// 条件句柄所有条件组
        /// </summary>
        IConditionGroupHandle Group { get; }

        /// <summary>
        /// 调用此方法触发条件句柄的更新(通过<see cref="OnUpdate"/>注册的事件)事件，
        /// 一般通过<see cref="IConditionCompare"/>实现类来触发。
        /// </summary>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        void Trigger(object oldValue, object newValue);

        /// <summary>
        /// 注册条件项更新事件回调
        /// <para>
        /// 若提前触发了更新事件，则会立即触发一次更新，并使用上次的值执行回调
        /// </para>
        /// </summary>
        /// <param name="callback">回调</param>
        void OnUpdate(Action<object, object> callback);

        /// <summary>
        /// 注册条件项完成事件回调
        /// <para>
        /// 当条件已经完成时，会立刻执行回调
        /// </para>
        /// </summary>
        /// <param name="callback">回调</param>
        void OnComplete(Action<IConditionHandle> callback);
    }
}
