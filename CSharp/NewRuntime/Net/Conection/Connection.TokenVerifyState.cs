using System;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class TokenVerifyState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.TokenVerify;

            private Guid _token;

            public override void OnInit()
            {
                base.OnInit();
                _token = Guid.Empty;
            }

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
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
                X.SystemLog.Debug("Net", $"create new client {_connection._id} {_token}");
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(token, true);

                if (result.State == NetOperateState.Cancel)
                {
                    AsyncEnd();
                    return;
                }

                ServerTokenVerify tokenVerify = result.Message as ServerTokenVerify;
                if (tokenVerify != null)
                {
                    X.SystemLog.Debug("Net", $"verify success {_connection._id} {new Guid(token.Id.Span)}");
                    ChangeState<RunState>().Forget();
                }
                else
                {
                    X.SystemLog.Debug("Net", $"tokenVerify error -> {_connection._client}");
                    ChangeState<CloseRequestState>().Forget();
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
                                responseHandle.SetResponse(messageResult);

                            MessageResult result = new MessageResult(messageResult.Message, _connection._stream);
                            _connection._onReceiveMessage?.Invoke(result);
                            return false;
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
