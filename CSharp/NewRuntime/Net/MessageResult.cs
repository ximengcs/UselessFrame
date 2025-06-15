
using System;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;

namespace UselessFrame.Net
{
    internal class MessageResult : IMessageResult
    {
        private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;
        private Guid _token;
        private Connection _connection;

        public IMessage Message { get; }
        public bool RequireResponse { get; }
        public Type MessageType { get; }

        public IConnection From => _connection;

        internal UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

        internal MessageResult(IMessage message, Connection connection)
        {
            Message = message;
            _connection = connection;

            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
            MessageType = typeInfo.Type;
            if (typeInfo.HasRequestToken)
            {
                _token = typeInfo.GetRequestToken(message);
                _responseTaskSource = AutoResetUniTaskCompletionSource<IMessage>.Create();
                RequireResponse = true;
            }
            else
            {
                _token = Guid.Empty;
                _responseTaskSource = null;
                RequireResponse = false;
            }
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
    }
}
