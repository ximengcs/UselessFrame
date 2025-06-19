
using Cysharp.Threading.Tasks;
using System.Net.Sockets;
using UselessFrame.Net;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class CloseRequestState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.CloseRequest;

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                TryRequest().Forget();
            }

            private async UniTask TryRequest()
            {
                AsyncBegin();

                ReadMessageResult result = await _connection._stream.SendWait(new CloseRequest(), true);

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            Socket socket = _connection._client.Client;
                            socket.Shutdown(SocketShutdown.Send);
                            _connection._stream.StartRead();
                            break;
                        }

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
                            break;
                        }
                }

                AsyncEnd();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.RemoteClose:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();
                            ChangeState<DisposeState>().Forget();
                            return false;
                        }

                    case NetOperateState.OK:
                        {
                            if (responseHandle.HasResponse)
                                responseHandle.SetCancel();
                            return true;
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
