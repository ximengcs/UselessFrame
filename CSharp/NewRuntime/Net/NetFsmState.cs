
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Text;
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Fiber;
using static UselessFrame.Net.Connection.MessageStream;

namespace UselessFrame.Net
{
    internal abstract class NetFsmState<T> where T : INetStateTrigger
    {
        public abstract int State { get; }

        private NetFsm<T> _fsm;
        private int _pendingCount;
        private bool _active;
        private AutoResetUniTaskCompletionSource _waitPendingSource;
        protected T _connection;

        public void OnInit(T connection, NetFsm<T> fsm)
        {
            _pendingCount = 0;
            _connection = connection;
            _fsm = fsm;
            OnInit();
        }

        public async UniTask ChangeState<ConnectionT>(MessageResult passMessage = default) where ConnectionT : NetFsmState<T>
        {
            if (!_active)
                return;

            _active = false;
            await _fsm.ChangeState(typeof(ConnectionT), passMessage);
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

        protected void CancelAllAsyncWait()
        {
            if (!_active)
                return;
            _connection.CancelAllAsyncWait();
        }

        protected string DebugPrefix => _connection.GetDebugPrefix(this);

        public virtual void OnInit()
        {
            X.SystemLog.Debug($"{DebugPrefix}-----Init-----");
        }

        public virtual void OnEnter(NetFsmState<T> preState, MessageResult passMessage)
        {
            _active = true;
            X.SystemLog.Debug($"{DebugPrefix}-----Enter-----");
        }

        public virtual void OnExit()
        {
            X.SystemLog.Debug($"{DebugPrefix}-----Exit-----\n");
        }

        public virtual void OnDispose()
        {
            X.SystemLog.Debug($"{DebugPrefix}-----Dispose-----");
        }

        public virtual async UniTask OnWaitEnd()
        {
            if (_pendingCount > 0)
            {
                _waitPendingSource = AutoResetUniTaskCompletionSource.Create();
                await _waitPendingSource.Task;
                _waitPendingSource = null;
            }
            await UniTask.Yield();
        }

        public async UniTask<bool> TriggerReceiveMessage(ReadMessageResult messageResult, WaitResponseHandle responseHandle)
        {
            if (!_active)
                return false;

            return await OnReceiveMessage(messageResult, responseHandle);
        }

        public virtual async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, WaitResponseHandle responseHandle)
        {
            string messageInfo = messageResult.Message != null ? messageResult.Message.ToString() : "null";
            X.SystemLog.Error($"{DebugPrefix}OnReceiveMessage state is error. receive state : {messageResult.State}, message : {messageInfo}, state message : {messageResult.StateMessage}");
            return false;
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
