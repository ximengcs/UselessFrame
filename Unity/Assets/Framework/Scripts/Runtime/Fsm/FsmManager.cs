using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime.StateMachine
{
    /// <inheritdoc/>
    public class FsmManager : IFsmManager, IManagerInitializer, IManagerDisposable, IManagerUpdater
    {
        #region Inner Field
        private Dictionary<string, IFsmBase> m_Fsms;
        private List<IFsmBase> m_FsmList;
        #endregion

        #region Module Life Fun
        public async UniTask Initialize(XSetting setting)
        {
            m_FsmList = new List<IFsmBase>();
            m_Fsms = new Dictionary<string, IFsmBase>();
        }

        /// <inheritdoc/>
        public void Update(float escapeTime)
        {
            foreach (IFsmBase fsm in m_FsmList)
                fsm.OnUpdate();
        }

        public void Dispose()
        {
            X.Log.Debug(FrameLogType.System, $"start dispose manager -> {GetType().Name}");
            foreach (IFsmBase fsm in m_FsmList)
                fsm.OnDestroy();
            X.Log.Debug(FrameLogType.System, $"dispose manager complete -> {GetType().Name}");
        }
        #endregion

        #region Interface
        /// <inheritdoc/>
        public IFsm GetOrNew(string name, Type[] states, IDataProvider dataProvider = null)
        {
            return InnerCreateFsm(name, states, dataProvider);
        }

        /// <inheritdoc/>
        public IFsm GetOrNew(Type[] states, IDataProvider dataProvider = null)
        {
            return GetOrNew(X.Random.NextString(8), states, dataProvider);
        }

        /// <inheritdoc/>
        public IFsm<T> GetOrNew<T>(string name, T owner, Type[] states, IDataProvider dataProvider = null)
        {
            return InnerCreateFsm(name, owner, states, dataProvider);
        }

        /// <inheritdoc/>
        public IFsm<T> GetOrNew<T>(T owner, Type[] states, IDataProvider dataProvider = null)
        {
            return GetOrNew(X.Random.NextString(8), owner, states, dataProvider);
        }

        /// <inheritdoc/>
        public void Remove(string name)
        {
            InnerRemoveFsm(name);
        }

        /// <inheritdoc/>
        public void Remove(IFsmBase fsm)
        {
            Remove(fsm.Name);
        }
        #endregion

        #region Inner Implement
        private IFsm<T> InnerCreateFsm<T>(string name, T owner, Type[] types, IDataProvider dataProvider)
        {
            List<FsmState<T>> states = new List<FsmState<T>>(types.Length);
            foreach (Type type in types)
            {
                FsmState<T> state = (FsmState<T>)X.Type.CreateInstance(type);
                states.Add(state);
            }

            IFsm<T> fsm = new Fsm<T>(name, states, owner, dataProvider);
            fsm.OnInit();
            m_Fsms[name] = fsm;
            m_FsmList.Add(fsm);
            return fsm;
        }

        private IFsm InnerCreateFsm(string name, Type[] types, IDataProvider dataProvider)
        {
            List<FsmState> states = new List<FsmState>(types.Length);
            foreach (Type type in types)
            {
                FsmState state = (FsmState)X.Type.CreateInstance(type);
                states.Add(state);
            }

            IFsm fsm = new Fsm(name, states, dataProvider);
            fsm.OnInit();
            m_Fsms[name] = fsm;
            m_FsmList.Add(fsm);
            return fsm;
        }

        private void InnerRemoveFsm(string name)
        {
            if (m_Fsms.TryGetValue(name, out IFsmBase fsm))
            {
                m_Fsms.Remove(name);
                m_FsmList.Remove(fsm);
                fsm.OnDestroy();
            }
        }
        #endregion
    } 
} 
