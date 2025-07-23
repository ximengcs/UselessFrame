using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    internal class NetFsm<T> where T : INetStateTrigger
    {
        private NetFsmState<T> _current;
        private T _connection;
        private Dictionary<Type, NetFsmState<T>> _states;

        public NetFsmState<T> Current => _current;

        public NetFsm(T owner, Dictionary<Type, NetFsmState<T>> types)
        {
            _connection = owner;
            _states = types;
            foreach (var item in types)
            {
                NetFsmState<T> state = item.Value;
                state.OnInit(owner, this);
            }
        }

        public void Start<TState>() where TState : NetFsmState<T>
        {
            ChangeState(typeof(TState)).Forget();
        }

        public async UniTask ChangeState(Type type, MessageResult passMessage = default)
        {
            await UniTask.Yield();
            NetFsmState<T> oldState = _current;
            if (oldState != null)
            {
                await oldState.OnWaitEnd();
                oldState.OnExit();
            }
            _current = _states[type];
            _connection.TriggerState(_current.State);
            _current.OnEnter(oldState, passMessage);
        }

        public void Dispose()
        {
            _current?.OnExit();
            foreach (var entry in _states)
                entry.Value.OnDispose();
        }
    }
}
