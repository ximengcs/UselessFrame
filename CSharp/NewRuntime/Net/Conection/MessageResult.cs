
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using static UselessFrame.Net.NetUtility;

namespace TestIMGUI.Core
{
    public struct MessageResult
    {
        private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;
        private Connection _connection;
        private Guid _token;

        public readonly IMessage Message;
        public readonly bool RequireResponse;

        internal UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

        internal MessageResult(IMessage message, Connection connection)
        {
            Message = message;
            _connection = connection;

            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
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
                _connection.Send(message).Forget();
            }
            else
            {
                X.SystemLog.Debug($"response message error");
                return;
            }
        }
    }
}
