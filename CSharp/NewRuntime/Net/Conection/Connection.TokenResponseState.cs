﻿
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class TokenResponseState : TokenCheck
        {
            public override int State => (int)ConnectionState.TokenResponse;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                if (passMessage != null)
                    SuccessHandler(passMessage).Forget();
                else
                    _connection._stream.StartRead();
            }

            private async UniTask<bool> SuccessHandler(MessageResult result)
            {
                ServerToken token = result.Message as ServerToken;
                _connection._id = token.GetId();
                X.SystemLog.Debug($"{DebugPrefix}receive server token {_connection._id}");
                bool success = await result.Response(new ServerTokenVerify() { ResponseToken = token.RequestToken });
                if (success)
                {
                    ChangeState<RunState>().Forget();
                }
                return success;
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            if (responseHandle.HasResponse)
                            {
                                responseHandle.SetResponse(messageResult);
                            }
                            else
                            {
                                MessageResult result = new MessageResult(messageResult.Message, _connection);
                                if (result.RequireResponse && result.MessageType == typeof(ServerToken))
                                {
                                    await SuccessHandler(result);
                                    return false;
                                }
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
