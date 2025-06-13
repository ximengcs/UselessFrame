
using Cysharp.Threading.Tasks;
using System;
using UselessFrame.Net;
using static NewConnection.ServerConnection.MessageStream;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal abstract class ConnectionState
        {
            private ConnectionFsm _fsm;
            private int _pendingCount;
            private AutoResetUniTaskCompletionSource _waitPendingSource;
            protected ServerConnection _connection;

            public void OnInit(ServerConnection connection, ConnectionFsm fsm)
            {
                _pendingCount = 0;
                _connection = connection;
                _fsm = fsm;
                OnInit();
            }

            public async UniTask ChangeState<T>() where T : ConnectionState
            {
                await _fsm.ChangeState(typeof(T));
            }

            protected void AsyncBegin()
            {
                _pendingCount++;
            }

            protected void AsyncEnd()
            {
                _pendingCount--;
                if (_pendingCount == 0 && _waitPendingSource != null)
                    _waitPendingSource.TrySetResult();
            }

            public virtual void OnInit() { }

            public virtual void OnEnter(ConnectionState preState) { }

            public virtual void OnExit() { }

            public virtual void OnDispose() { }

            public virtual async UniTask OnWaitEnd()
            {
                _waitPendingSource = AutoResetUniTaskCompletionSource.Create();
                await _waitPendingSource.Task;
                _waitPendingSource = null;
            }

            public virtual UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, WaitResponseHandle responseHandle)
            {
                throw new NotImplementedException();
            }
        }
    }

}
