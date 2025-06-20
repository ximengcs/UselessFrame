﻿
using Google.Protobuf;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class RunState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Run;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);
                _connection._stream.StartRead();
                _connection._stream.SetWriteActive(true);
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
                            MessageResult result = new MessageResult(messageResult.Message, _connection);
                            return result;
                        }

                    default:
                        {
                            return null;
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
                            }
                            else
                            {
                                MessageResult result = new MessageResult(messageResult.Message, _connection);
                                if (result.RequireResponse && result.MessageType == typeof(CloseRequest))
                                {
                                    ChangeState<CloseResponseState>(result).Forget();
                                    CancelAllAsyncWait();
                                    return false;
                                }
                                _connection.TriggerNewMessage(result);
                            }
                            return true;
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
        }
    }
}
