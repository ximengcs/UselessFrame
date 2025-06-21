
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

            public override void OnEnter(NetFsmState<Connection> preState, MessageResult passMessage)
            {
                base.OnEnter(preState, passMessage);

                if (passMessage != null)
                    TryResponse(passMessage).Forget();
                else
                    ChangeState<DisposeState>().Forget();
            }

            private async UniTask TryResponse(MessageResult passMessage)
            {
                AsyncBegin();

                X.SystemLog.Debug($"{DebugPrefix}try response close");
                bool success = await passMessage.Response(new CloseResponse());
                X.SystemLog.Debug($"{DebugPrefix}try response close complete");
                try
                {
                    Socket socket = _connection._client.Client;
                    X.SystemLog.Debug($"{DebugPrefix}shutdown send");
                    socket.Shutdown(SocketShutdown.Send);
                    X.SystemLog.Debug($"{DebugPrefix}try read finish");
                    _connection._stream.StartRead();
                }
                catch (ObjectDisposedException e)
                {
                    X.SystemLog.Error(e);
                    ChangeState<DisposeState>().Forget();
                }

                AsyncEnd();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.RemoteClose:
                        {
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
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
                            ChangeState<DisposeState>().Forget();
                            CancelAllAsyncWait();
                            return false;
                        }
                }
            }
        }
    }

}
