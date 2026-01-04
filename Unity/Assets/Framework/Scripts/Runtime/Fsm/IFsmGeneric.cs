using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime.StateMachine
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <typeparam name="T">状态机拥有者类型</typeparam>
    public interface IFsm<T> : IFsmBase
    {
        /// <summary>
        /// 状态机拥有者
        /// </summary>
        T Owner { get; }

        /// <summary>
        /// 当前状态
        /// </summary>
        FsmState<T> Current { get; }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="State">状态类型</typeparam>
        /// <returns>获取到的状态实例</returns>
        State GetState<State>() where State : FsmState<T>;

        /// <summary>
        /// 是否含有状态
        /// </summary>
        /// <typeparam name="State">状态类型</typeparam>
        /// <returns>true表示含有</returns>
        bool HasState<State>() where State : FsmState<T>;

        /// <summary>
        /// 启动状态机
        /// </summary>
        /// <typeparam name="State">入口状态类型</typeparam>
        void Start<State>() where State : FsmState<T>;
    }
}
