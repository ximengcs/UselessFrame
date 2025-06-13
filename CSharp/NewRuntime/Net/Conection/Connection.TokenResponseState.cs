
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class TokenResponseState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.TokenResponse;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                _connection._stream.StartRead();
            }

            private async UniTask TryReponseVerify(ServerToken token)
            {
                AsyncBegin();

                ServerTokenVerify verify = new ServerTokenVerify() { ResponseToken = token.RequestToken };
                WriteMessageResult writeResult = await _connection._stream.Send(verify, true);
                switch (writeResult.State)
                {
                    case NetOperateState.OK:
                        ChangeState<RunState>().Forget();
                        break;

                    default:
                        ChangeState<CloseRequestState>().Forget();
                        break;
                }

                AsyncEnd();
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
                                MessageResult result = new MessageResult(messageResult.Message, _connection._stream);
                                if (result.RequireResponse && result.MessageType == typeof(ServerToken))
                                {
                                    ServerToken token = result.Message as ServerToken;
                                    _connection._id = token.GetId();
                                    await TryReponseVerify(token);
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
