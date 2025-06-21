
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using System.Data.Common;
using UselessFrame.NewRuntime;
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
                                    return false;
                                }
                                _connection.TriggerNewMessage(result);
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

                            ChangeState<DisposeState>().Forget();
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
