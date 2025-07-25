﻿
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

                if (passMessage.Valid)
                    TryResponse(passMessage).Forget();
                else
                    ChangeState<DisposeState>().Forget();
            }

            private async UniTask TryResponse(MessageResult passMessage)
            {
                AsyncBegin();

                X.Log.Debug($"{DebugPrefix}try response close");
                bool success = await passMessage.Response(new CloseResponse());
                X.Log.Debug($"{DebugPrefix}try response close complete");
                try
                {
                    Socket socket = _connection._client.Client;
                    X.Log.Debug($"{DebugPrefix}shutdown send");
                    socket.Shutdown(SocketShutdown.Send);
                    X.Log.Debug($"{DebugPrefix}try read finish");
                    _connection._stream.StartRead();
                }
                catch (ObjectDisposedException e)
                {
                    X.Log.Error(e);
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
