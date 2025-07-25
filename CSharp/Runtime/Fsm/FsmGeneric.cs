using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime.StateMachine
{
    internal class Fsm<T> : IFsm<T>
    {
        private T m_Owner;
        private string m_Name;
        private Dictionary<Type, FsmState<T>> m_States;
        private FsmState<T> m_Current;
        private IDataProvider _data;

        public T Owner => m_Owner;
        public string Name => m_Name;
        public FsmState<T> Current => m_Current;

        public IDataProvider Data { get; set; }

        public Fsm(string name, List<FsmState<T>> states, T owner, IDataProvider dataProvider)
        {
            m_Name = name;
            m_Owner = owner;
            m_Current = null;
            _data = dataProvider;
            m_States = new Dictionary<Type, FsmState<T>>();
            foreach (FsmState<T> state in states)
                m_States[state.GetType()] = state;
        }

        public TState GetState<TState>() where TState : FsmState<T>
        {
            if (m_States.TryGetValue(typeof(TState), out FsmState<T> state))
                return (TState)state;
            else
                return default;
        }

        public bool HasState<TState>() where TState : FsmState<T>
        {
            return m_States.ContainsKey(typeof(TState));
        }

        public void Start<TState>() where TState : FsmState<T>
        {
            if (HasState<TState>())
            {
                ChangeState<TState>();
            }
        }

        internal void ChangeState<TState>() where TState : FsmState<T>
        {
            m_Current?.OnLeave();
            if (m_States.TryGetValue(typeof(TState), out FsmState<T> state))
            {
                m_Current = state;
                m_Current.OnEnter();
            }
        }

        void IFsmBase.OnInit()
        {
            foreach (FsmState<T> state in m_States.Values)
                state.OnInit(this);
        }

        void IFsmBase.OnUpdate()
        {
            m_Current?.OnUpdate();
        }

        void IFsmBase.OnDestroy()
        {
            foreach (FsmState<T> state in m_States.Values)
                state.OnDestroy();
            m_States = null;
            m_Current = null;
        }
    }
}
