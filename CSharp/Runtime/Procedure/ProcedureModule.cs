using Cysharp.Threading.Tasks;
using System;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.StateMachine;

namespace XFrame.Modules.Procedure
{
    public class ProcedureModule : IProcedureModule, IManagerInitializer
    {
        #region Inner Fields
        private IFsm m_Fsm;
        private Type _startProc;
        #endregion

        #region Life Fun
        public async UniTask Initialize(XSetting setting)
        {
            _startProc = setting.EntranceProcedure;
            m_Fsm = X.Fsm.GetOrNew(X.Type.GetCollection(typeof(ProcedureBase)).ToArray());
        }

        public void Start()
        {
            if (_startProc != null)
                m_Fsm.Start(_startProc);
        }
        #endregion

        #region Interface
        /// <inheritdoc/>
        public ProcedureBase Current => (ProcedureBase)m_Fsm.Current;
        #endregion
    }
}
