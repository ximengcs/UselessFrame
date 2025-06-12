
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UselessFrame.Net;

namespace NewConnection
{
    internal abstract class ConnectionState
    {
        private ConnectionFsm _fsm;
        private int _pendingCount;
        private AutoResetUniTaskCompletionSource _waitPendingSource;

        protected bool _waitCancel;
        protected bool _stateTransitioning;
        protected ServerConnection _connection;

        public void OnInit(ServerConnection connection, ConnectionFsm fsm)
        {
            _stateTransitioning = false;
            _pendingCount = 0;
            _connection = connection;
            _fsm = fsm;
            OnInit();
        }

        public async UniTask ChangeState<T>() where T : ConnectionState
        {
            _stateTransitioning = true;
            await _fsm.ChangeState(typeof(T));
            _stateTransitioning = false;
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
            _waitCancel = true;
            _waitPendingSource = AutoResetUniTaskCompletionSource.Create();
            await _waitPendingSource.Task;
            _waitPendingSource = null;
            _waitCancel = false;
        }

        public virtual void OnReceiveMessage(ReadMessageResult messageResult) { }
    }
}
