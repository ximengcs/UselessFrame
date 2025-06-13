
using System;
using Google.Protobuf;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.Connection.MessageStream;

namespace UselessFrame.Net
{
    internal abstract class NetFsmState<T> where T : INetStateTrigger
    {
        public abstract int State { get; }

        private NetFsm<T> _fsm;
        private int _pendingCount;
        private AutoResetUniTaskCompletionSource _waitPendingSource;
        protected T _connection;

        public void OnInit(T connection, NetFsm<T> fsm)
        {
            _pendingCount = 0;
            _connection = connection;
            _fsm = fsm;
            OnInit();
        }

        public async UniTask ChangeState<ConnectionT>() where ConnectionT : NetFsmState<T>
        {
            await _fsm.ChangeState(typeof(ConnectionT));
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

        public virtual void OnEnter(NetFsmState<T> preState) { }

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

        public virtual async UniTask OnSendMessage(IMessage message)
        {
            await UniTask.CompletedTask;
        }

        public virtual async UniTask<MessageResult> OnSendWaitMessage(IMessage message)
        {
            await UniTask.CompletedTask;
            return default;
        }
    }

}
