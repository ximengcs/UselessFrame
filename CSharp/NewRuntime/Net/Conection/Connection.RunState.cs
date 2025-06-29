
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;
using System;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class RunState : NetFsmState<Connection>
        {
            private MessageBeat _beat;

            public override int State => (int)ConnectionState.Run;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                if (_beat == null)
                {
                    _beat = new MessageBeat(_connection._runFiber, _connection._stream, 60);
                    _beat.ErrorEvent += OnErrorHandler;
                }

                _connection._stream.StartRead();
                _connection._stream.SetWriteActive(true);
                _beat.Start();
            }

            public override void OnExit()
            {
                base.OnExit();
                if (_beat != null)
                {
                    _beat.Stop();
                }
            }

            public override void OnDispose()
            {
                base.OnDispose();
                if (_beat != null)
                {
                    _beat.Dispose();
                    _beat = null;
                }
            }

            public override async UniTask OnSendMessage(IMessage message, IFiber fiber)
            {
                WriteMessageResult result = await _connection.Stream.Send(message, false, fiber);
                switch (result.State)
                {
                    case NetOperateState.OK:
                        break;

                    default:
                        break;
                }
            }

            public override async UniTask<MessageResult> OnSendWaitMessage(IMessage message, IFiber fiber)
            {
                ReadMessageResult messageResult = await _connection.Stream.SendWait(message, false, fiber);
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            MessageResult result = MessageResult.Create(messageResult.Message, _connection);
                            return result;
                        }

                    default:
                        {
                            return default;
                        }
                }
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
                                if (result.MessageType == typeof(CloseRequest))
                                {
                                    ChangeState<CloseResponseState>(result).Forget();
                                    CancelAllAsyncWait();
                                    return false;
                                }
                                if (result.MessageType == typeof(KeepAlive))
                                {
                                    if (_connection.GetRuntimeData<ConnectionSetting>().ShowReceiveKeepaliveLog)
                                        X.SystemLog.Debug($"{DebugPrefix}receive keepalive.");
                                    return true;
                                }
                                if (result.MessageType == typeof(CommandMessage))
                                {
                                    CommandMessage cmd = (CommandMessage)result.Message;
                                    X.SystemLog.Debug($"{DebugPrefix}execute command -> {cmd.CommandStr}.");
                                    _connection._dataFiber.Post(ToFiberFun.RunCommand, Tuple.Create(_connection, result));
                                    return true;
                                }
                                _connection.TriggerNewMessage(result);
                                return true;
                            }
                        }

                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}receive message happend socket error, {messageResult.Exception.ErrorCode}");
                            X.SystemLog.Exception(messageResult.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }

                    default:
                        {
                            X.SystemLog.Debug($"{DebugPrefix}receive message error, {messageResult.State} {messageResult.StateMessage}");
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }

            private void OnErrorHandler(WriteMessageResult result)
            {
                switch (result.State)
                {
                    case NetOperateState.SocketError:
                        {
                            X.SystemLog.Error($"{DebugPrefix}send keepalive happend socket error, {result.Exception.ErrorCode}");
                            X.SystemLog.Exception(result.Exception);
                            ChangeState<CheckConnectState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }

                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }

                    default:
                        {
                            X.SystemLog.Debug($"{DebugPrefix}send keepalive message error, {result.State} {result.StateMessage}");

                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            break;
                        }
                }
            }
        }
    }
}
