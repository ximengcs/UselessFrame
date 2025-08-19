using System;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class TokenVerifyState : TokenCheck
        {
            public override int State => (int)ConnectionState.TokenVerify;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _connection.Stream.StartRead();
            }

            private async UniTask Verify()
            {
                AsyncBegin();
                ServerToken token = NetUtility.CreateToken(_connection.Id);
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}send verify token");
                _connection._stream.StartRead();
                ReadMessageResult result = await _connection._stream.SendWait(token, true);
                X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}send verify token complete, state {result.State}");

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            ServerTokenVerify tokenVerify = (ServerTokenVerify)result.Message;
                            X.Log.Debug(FrameLogType.Net, $"{DebugPrefix}verify success");
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

                AsyncEnd();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                            if (responseHandle.HasResponse)
                            {
                                if (result.MessageType == typeof(ServerTokenVerify))
                                {
                                    responseHandle.SetResponse(messageResult);
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                if (_messageHandler.TryGetValue(result.MessageType, out var handler))
                                {
                                    return handler(result);
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }

                    case NetOperateState.SocketError:
                        {
                            X.Log.Error($"{DebugPrefix}token verify happend socket error, {messageResult.Exception.ErrorCode}");
                            X.Log.Exception(messageResult.Exception);
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
