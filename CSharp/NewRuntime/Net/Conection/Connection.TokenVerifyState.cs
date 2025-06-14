﻿using System;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class TokenVerifyState : TokenCheck
        {
            public override int State => (int)ConnectionState.TokenVerify;

            private Guid _token;

            public override void OnInit()
            {
                base.OnInit();
                _token = Guid.Empty;
            }

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                if (_token != Guid.Empty)
                {
                    ChangeState<RunState>().Forget();
                }
                else
                {
                    Verify().Forget();
                }
            }

            private async UniTask Verify()
            {
                AsyncBegin();

                ServerToken token = NetUtility.CreateToken(_connection._id);
                _token = new Guid(token.Id.Span);
                X.SystemLog.Debug($"{DebugPrefix}send verify token");
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(token, true);
                X.SystemLog.Debug($"{DebugPrefix}send verify token complete, state {result.State}");
                if (result.State == NetOperateState.Cancel)
                {
                    AsyncEnd();
                    return;
                }

                ServerTokenVerify tokenVerify = (ServerTokenVerify)result.Message;
                X.SystemLog.Debug($"{DebugPrefix}verify success");
                ChangeState<RunState>().Forget();

                AsyncEnd();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = new MessageResult(messageResult.Message, _connection);
                            if (result.MessageType == typeof(ServerTokenVerify))
                            {
                                responseHandle.SetResponse(messageResult);
                                return false;
                            }
                            else
                            {
                                if (responseHandle.HasResponse)
                                    responseHandle.SetResponse(messageResult);
                            }

                            return true;
                        }

                    case NetOperateState.SocketError:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            X.SystemLog.Debug($"verify socket error {messageResult.Exception.ErrorCode}");
                            ChangeState<CheckConnectState>().Forget();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            ChangeState<CloseResponseState>().Forget();
                            return false;
                        }

                    default:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            ChangeState<DisposeState>().Forget();
                            return false;
                        }
                }
            }
        }
    }
}
