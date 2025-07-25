using System;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime.StateMachine
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public interface IFsm : IFsmBase
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        FsmState Current { get; }

        IDataProvider Data { get; }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="State">状态类型</typeparam>
        /// <returns>获取到的状态实例</returns>
        State GetState<State>() where State : FsmState;

        /// <summary>
        /// 是否含有状态
        /// </summary>
        /// <typeparam name="State">状态类型</typeparam>
        /// <returns>true表示含有</returns>
        bool HasState<State>() where State : FsmState;

        /// <summary>
        /// 是否含有状态
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <returns>true表示含有</returns>
        bool HasState(Type type);

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <typeparam name="State">入口状态类型</typeparam>
        void Start<State>() where State : FsmState;

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <param name="type">入口状态类型</param>
        void Start(Type type);
    }
}
