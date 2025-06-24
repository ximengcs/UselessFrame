using System;
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

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            ServerTokenVerify tokenVerify = (ServerTokenVerify)result.Message;
                            X.SystemLog.Debug($"{DebugPrefix}verify success");
                            ChangeState<RunState>().Forget();
                        }
                        break;

                    case NetOperateState.Cancel:
                        break;

                    case NetOperateState.Timeout:
                        {
                            ChangeState<DisposeState>().Forget();
                        }
                        break;

                    default:
                        {
                            ChangeState<DisposeState>().Forget();
                        }
                        break;
                }
                result.Dispose();

                AsyncEnd();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                            if (result.MessageType == typeof(ServerTokenVerify))
                            {
                                responseHandle.SetResponse(messageResult);
                                return false;
                            }
                            else
                            {
                                messageResult.Dispose();
                                responseHandle.SetCancel();
                            }
                            result.Dispose();
                            return true;
                        }

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}token verify happend socket error, {messageResult.Exception.ErrorCode}");
                            X.SystemLog.Exception(messageResult.Exception);
                            messageResult.Dispose();
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            messageResult.Dispose();
                            ChangeState<CloseResponseState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    default:
                        {
                            messageResult.Dispose();
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }
        }
    }
}
