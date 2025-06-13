
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class RunState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.Run;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                _connection._stream.StartRead();
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
                                if (result.RequireResponse && result.MessageType == typeof(CloseRequest))
                                {
                                    ChangeState<CloseResponseState>().Forget();
                                    return false;
                                }
                                _connection._onReceiveMessage?.Invoke(result);
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
