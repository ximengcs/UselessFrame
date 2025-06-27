
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
                result.Dispose();
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
                                return true;
                            }
                            else
                            {
                                MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                                if (result.RequireResponse && result.MessageType == typeof(ServerToken))
                                {
                                    await SuccessHandler(result);
                                    return false;
                                }
                                else
                                {
                                    result.Dispose();
                                }
                                return true;
                            }
                        }

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}token response happend socket error, {messageResult.Exception.ErrorCode}");
                            X.SystemLog.Exception(messageResult.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<CloseResponseState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    default:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }
        }
    }
}
