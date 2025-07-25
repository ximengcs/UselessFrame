﻿
using System;
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;

namespace UselessFrame.Net
{
    public struct MessageResult
    {
        private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;
        private int _token;
        private Connection _connection;
        private IMessage _message;
        private Type _messageType;
        private bool _requireResponse;
        private bool _valid;

        public bool Valid => _valid;
        public IMessage Message => _message;
        public bool RequireResponse => _requireResponse;
        public Type MessageType => _messageType;

        public IConnection From => _connection;

        internal UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

        internal static MessageResult Create(IMessage message, Connection connection)
        {
            MessageResult result = new MessageResult();
            result._valid = true;
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
                result._token = 0;
                result._responseTaskSource = null;
                result._requireResponse = false;
            }
            return result;
        }

        public async UniTask<bool> Response(IMessage message)
        {
            if (!RequireResponse)
            {
                X.Log.Debug($"can not require response");
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
                            X.Log.Error(result.StateMessage);
                            _connection.Fsm.ChangeState(typeof(CloseRequest)).Forget();
                            return false;
                        }
                }
            }
            else
            {
                X.Log.Debug($"response message error");
                return false;
            }
        }
    }
}
