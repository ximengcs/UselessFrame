using XFrame.Utility;
using XFrame.Modules.Diagnotics;
using XFrame.Modules.StateMachine;

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
            m_InstName = TypeUtility.GetSimpleName(GetType().Name);
        }

        /// <inheritdoc/>
        protected internal override void OnEnter()
        {
            base.OnEnter();
            Log.Debug(Log.Procedure, $"Enter {m_InstName} Procedure");
        }

        /// <inheritdoc/>
        protected internal override void OnLeave()
        {
            base.OnLeave();
            Log.Debug(Log.Procedure, $"Leave {m_InstName} Procedure");
        }
    }
}
