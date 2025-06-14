
using System;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;
using static UselessFrame.Net.Connection;

namespace UselessFrame.Net
{
    internal class MessageResult : IMessageResult
    {
        private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;
        private Guid _token;

        public IMessage Message { get; }
        public bool RequireResponse { get; }
        public Type MessageType { get; }

        public IConnection From { get; }

        internal UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

        internal MessageResult(IMessage message, Connection connection)
        {
            Message = message;
            From = connection;

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

        public void Response(IMessage message)
        {
            if (!RequireResponse)
            {
                X.SystemLog.Debug($"can not require response");
                return;
            }

            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
            if (typeInfo.HasRequestToken)
            {
                typeInfo.SetRequestToken(message, _token);
                From.Send(message);
            }
            else
            {
                X.SystemLog.Debug($"response message error");
                return;
            }
        }
    }
}
