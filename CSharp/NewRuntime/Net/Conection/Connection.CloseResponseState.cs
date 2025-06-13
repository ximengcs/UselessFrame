
using Cysharp.Threading.Tasks;
using System;
using System.Net.Sockets;
using UselessFrame.Net;
using UselessFrame.NewRuntime;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal class CloseResponseState : NetFsmState<Connection>
        {
            public override int State => (int)ConnectionState.CloseResponse;

            public override void OnEnter(NetFsmState<Connection> preState)
            {
                base.OnEnter(preState);
                TryResponse().Forget();
            }

            private async UniTask TryResponse()
            {
                AsyncBegin();

                WriteMessageResult result = await _connection._stream.Send(new CloseResponse(), true);

                switch (result.State)
                {
                    case NetOperateState.OK:
                        {
                            try
                            {
                                Socket socket = _connection._client.Client;
                                socket.Shutdown(SocketShutdown.Send);
                                _connection._stream.StartRead();
                            }
                            catch (ObjectDisposedException e)
                            {
                                X.SystemLog.Error(e);
                                ChangeState<DisposeState>().Forget();
                            }
                            break;
                        }

                    case NetOperateState.Cancel:
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
