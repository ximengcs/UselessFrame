using Cysharp.Threading.Tasks;
using System;
using TestIMGUI.Core;
using UselessFrame.Net;
using UselessFrame.NewRuntime;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal class VerifyTokenState : ConnectionState
        {
            private Guid _token;
            private int _tryTimes;

            public override void OnInit()
            {
                base.OnInit();
                _token = Guid.Empty;
            }

            public override void OnEnter(ConnectionState preState)
            {
                base.OnEnter(preState);
                _tryTimes = 3;
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
                    if (_tryTimes > 0)
                    {
                        Verify().Forget();
                    }
                    else
                    {
                        ChangeState<CloseRequestState>().Forget();
                    }
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

                    case NetOperateState.FatalError:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            ChangeState<CloseRequestState>().Forget();
                            return false;
                        }

                    case NetOperateState.SocketError:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            X.SystemLog.Debug($"verify socket error {messageResult.Exception.ErrorCode}");
                            ChangeState<CloseRequestState>().Forget();
                            return false;
                        }

                    case NetOperateState.CloseRequest:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();

                            ChangeState<CloseResponseState>().Forget();
                            return false;
                        }

                    default: return false;
                }
            }
        }
    }
}
