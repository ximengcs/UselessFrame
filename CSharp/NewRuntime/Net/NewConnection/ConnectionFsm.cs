using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace NewConnection
{
    internal class ConnectionFsm
    {
        private ConnectionState _current;
        private Dictionary<Type, ConnectionState> _states;

        public ConnectionState Current => _current;

        public ConnectionFsm(ServerConnection owner, ConnectionState[] types)
        {
            _states = new Dictionary<Type, ConnectionState>();
            foreach (ConnectionState t in types)
            {
                t.OnInit(owner, this);
                _states.Add(t.GetType(), t);
            }
        }

        public void Start<T>() where T : ConnectionState
        {
            ChangeState(typeof(T)).Forget();
        }

        public async UniTask ChangeState(Type type)
        {
            ConnectionState oldState = _current;
            if (oldState != null)
            {
                await oldState.OnWaitEnd();
                oldState.OnExit();
            }
            _current = _states[type];
            _current.OnEnter(oldState);
        }

        public void Dispose()
        {
            _current?.OnExit();
            foreach (var entry in _states)
                entry.Value.OnDispose();
        }
    }
}
