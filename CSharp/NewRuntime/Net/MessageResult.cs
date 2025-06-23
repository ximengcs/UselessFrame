
using System;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;

namespace UselessFrame.Net
{
    internal class MessageResult : IMessageResult, INetPoolObject, IDisposable
    {
        private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;
        private Guid _token;
        private Connection _connection;
        private IMessage _message;
        private Type _messageType;
        private bool _requireResponse;

        public IMessage Message => _message;
        public bool RequireResponse => _requireResponse;
        public Type MessageType => _messageType;

        public IConnection From => _connection;

        internal UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

        internal static MessageResult Create(IMessage message, Connection connection)
        {
            MessageResult result = NetPoolUtility._messageResultPool.Require();
            result._message = message;
            result._connection = connection;

            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
            result._messageType = typeInfo.Type;
            if (typeInfo.HasRequestToken)
            {
                result._token = typeInfo.GetRequestToken(message);
                result._responseTaskSource = AutoResetUniTaskCompletionSource<IMessage>.Create();
                result._requireResponse = true;
            }
            else
            {
                result._token = Guid.Empty;
                result._responseTaskSource = null;
                result._requireResponse = false;
            }
            return result;
        }

        public async UniTask<bool> Response(IMessage message)
        {
            if (!RequireResponse)
            {
                X.SystemLog.Debug($"can not require response");
                return false;
            }

            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
            if (typeInfo.HasResponseToken)
            {
                typeInfo.SetResponseToken(message, _token);
                WriteMessageResult result = await _connection.Stream.Send(message, true);
                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            return true;
                        }

                    default:
                        {
                            X.SystemLog.Error(result.StateMessage);
                            _connection.Fsm.ChangeState(typeof(CloseRequest)).Forget();
                            return false;
                        }
                }
            }
            else
            {
                X.SystemLog.Debug($"response message error");
                return false;
            }
        }

        public void Reset()
        {
            _responseTaskSource = null;
            _connection = null;
            _message = null;
            _requireResponse = false;
            _messageType = null;
        }

        public void Dispose()
        {
            NetPoolUtility._messageResultPool.Release(this);
        }
    }
}
