
namespace UselessFrame.NewRuntime.StateMachine
{
    /// <summary>
    /// 有限状态机状态
    /// </summary>
    public abstract class FsmState<T>
    {
        /// <summary>
        /// 持有状态机
        /// </summary>
        protected IFsm<T> m_Fsm;

        /// <summary>
        /// 初始化生命周期
        /// </summary>
        /// <param name="fsm">状态机</param>
        protected internal virtual void OnInit(IFsm<T> fsm)
        {
            m_Fsm = fsm;
        }

        /// <summary>
        /// 进入状态生命周期
        /// </summary>
        protected internal virtual void OnEnter()
        {

        }

        /// <summary>
        /// 更新生命周期
        /// </summary>
        protected internal virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 离开状态生命周期
        /// </summary>
        protected internal virtual void OnLeave()
        {

        }

        /// <summary>
        /// 销毁生命周期
        /// </summary>
        protected internal virtual void OnDestroy()
        {
            m_Fsm = null;
        }

        /// <summary>
        /// 改变状态机状态
        /// 仅通过内部调用
        /// </summary>
        /// <typeparam name="State">状态机类型</typeparam>
        protected internal void ChangeState<State>() where State : FsmState<T>
        {
            Fsm<T> realFsm = m_Fsm as Fsm<T>;
            realFsm.ChangeState<State>();
        }
    }
}
