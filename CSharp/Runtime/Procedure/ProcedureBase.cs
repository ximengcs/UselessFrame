using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.StateMachine;

namespace XFrame.Modules.Procedure
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class ProcedureBase : FsmState
    {
        private string m_InstName;

        /// <inheritdoc/>
        protected internal override void OnInit(IFsm fsm)
        {
            base.OnInit(fsm);
            m_InstName = GetType().Name;
        }

        /// <inheritdoc/>
        protected internal override void OnEnter()
        {
            base.OnEnter();
            X.Log.Debug(FrameLogType.Procedure, $"Enter {m_InstName} Procedure");
        }

        /// <inheritdoc/>
        protected internal override void OnLeave()
        {
            base.OnLeave();
            X.Log.Debug(FrameLogType.Procedure, $"Leave {m_InstName} Procedure");
        }
    }
}
