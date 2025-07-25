using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime.StateMachine
{
    internal class Fsm : IFsm
    {
        #region Inner Fields
        private string m_Name;
        private Dictionary<Type, FsmState> m_States;
        private FsmState m_Current;
        private IDataProvider _data;
        #endregion

        public string Name => m_Name;
        public FsmState Current => m_Current;

        public IDataProvider Data => _data;

        internal void InnerAddState(FsmState state)
        {
            m_States[state.GetType()] = state;
            state.OnInit(this);
        }

        public TState GetState<TState>() where TState : FsmState
        {
            if (m_States.TryGetValue(typeof(TState), out FsmState state))
                return (TState)state;
            else
                return default;
        }

        public bool HasState<TState>() where TState : FsmState
        {
            return HasState(typeof(TState));
        }

        public bool HasState(Type type)
        {
            return m_States.ContainsKey(type);
        }

        public void Start<TState>() where TState : FsmState
        {
            if (HasState<TState>())
            {
                ChangeState(typeof(TState));
            }
        }

        public void Start(Type type)
        {
            if (HasState(type))
            {
                ChangeState(type);
            }
        }

        public Fsm(string name, List<FsmState> states, IDataProvider dataProvider)
        {
            m_Name = name;
            m_Current = null;
            _data = dataProvider;
            m_States = new Dictionary<Type, FsmState>();
            foreach (FsmState state in states)
                m_States[state.GetType()] = state;
        }

        public void ChangeState(Type type)
        {
            m_Current?.OnLeave();
            if (m_States.TryGetValue(type, out FsmState state))
            {
                m_Current = state;
                m_Current.OnEnter();
            }
        }

        void IFsmBase.OnInit()
        {
            foreach (FsmState state in m_States.Values)
                state.OnInit(this);
        }

        void IFsmBase.OnUpdate()
        {
            m_Current?.OnUpdate();
        }

        void IFsmBase.OnDestroy()
        {
            foreach (FsmState state in m_States.Values)
                state.OnDestroy();
            m_States = null;
            m_Current = null;
        }
    }
}
