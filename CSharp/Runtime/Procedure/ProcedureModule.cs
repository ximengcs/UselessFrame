using System;
using XFrame.Core;
using XFrame.Utility;
using XFrame.Modules.Reflection;
using XFrame.Modules.Config;
using XFrame.Modules.Diagnotics;
using XFrame.Modules.StateMachine;
using XFrame.Collections;

namespace XFrame.Modules.Procedure
{
    /// <inheritdoc/>
    [CommonModule]
    [XType(typeof(IProcedureModule))]
    public class ProcedureModule : ModuleBase, IProcedureModule
    {
        #region Inner Fields
        private TypeSystem m_Procedures;
        private Fsm m_Fsm;
        #endregion

        #region Life Fun
        /// <inheritdoc/>
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            InnerRefreshHandler();
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            base.OnStart();
            string entrance = XConfig.Entrance;
            if (!string.IsNullOrEmpty(entrance) && m_Procedures.TryGetByName(entrance, out Type type))
            {
                m_Fsm = (Fsm)Domain.GetModule<IFsmModule>().GetOrNew(m_Procedures.ToArray());
                m_Fsm.Start(type);
            }
            else
            {
                Log.Error(Log.XFrame, $"{TypeUtility.GetSimpleName(entrance)} Procedure do not define");
            }
        }
        #endregion

        #region Interface
        /// <inheritdoc/>
        public IFsm Fsm => m_Fsm;

        /// <inheritdoc/>
        public void Redirect(string name)
        {
            InnerRefreshHandler();
            if (!string.IsNullOrEmpty(name) && m_Procedures.TryGetByName(name, out Type type))
            {
                Log.Debug(Log.XFrame, $"Enter {TypeUtility.GetSimpleName(name)} Procedure");
                m_Fsm.ChangeState(type);
            }
            else
            {
                Log.Error(Log.XFrame, $"{TypeUtility.GetSimpleName(name)} Procedure do not define");
            }
        }

        /// <inheritdoc/>
        public void Redirect(Type type)
        {
            if (m_Fsm != null && m_Fsm.HasState(type))
                m_Fsm.ChangeState(type);
        }

        /// <inheritdoc/>
        public void Add(Type type)
        {
            InnerAdd(type);
        }

        /// <inheritdoc/>
        public void Add<T>() where T : ProcedureBase
        {
            InnerAdd(typeof(T));
        }
        #endregion

        #region Inner Imeplement
        private void InnerAdd(Type type)
        {
            if (!m_Fsm.HasState(type))
            {
                ProcedureBase proc = (ProcedureBase)Domain.TypeModule.CreateInstance(type);
                m_Fsm.InnerAddState(proc);
            }
            else
            {
                Log.Debug(Log.XFrame, $"Already has exist proc {type.Name}.");
            }
        }

        private void InnerRefreshHandler()
        {
            m_Procedures = Domain.TypeModule.GetOrNew<ProcedureBase>();
        }
        #endregion
    }
}
