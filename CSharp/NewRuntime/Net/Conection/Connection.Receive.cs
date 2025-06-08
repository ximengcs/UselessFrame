using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using static UselessFrame.Net.NetUtility;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private async UniTaskVoid RequestMessage()
        {
            ReadMessageResult result = await MessageUtility.ReadMessageAsync(_client, _pool, _closeTokenSource.Token);
            if (_state.Value == ConnectionState.Normal)
            {
                switch (result.State)
                {
                    case NetOperateState.OK:
                        SuccessHandler(result);
                        RequestMessage().Forget();
                        break;

                    case NetOperateState.InValidRequest:
                        X.SystemLog.Debug("Net", $"reqeust message happend invalid {result.State} {result.StateMessage}");
                        RequestMessage().Forget();
                        break;

                    case NetOperateState.NormalClose:
                        _state.Value = ConnectionState.NormalClose;
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.Disconnect:
                    case NetOperateState.DataError:
                    case NetOperateState.ParamError:
                    case NetOperateState.PermissionError:
                    case NetOperateState.RemoteClose:
                    case NetOperateState.Unknown:
                        X.SystemLog.Debug("Net", $"request message error {result.State} {result.StateMessage}");
                        _state.Value = ConnectionState.FatalErrorClose;
                        _closeTokenSource.Cancel();
                        Dispose();
                        break;

                    case NetOperateState.SocketError:
                        X.SystemLog.Debug("Net", $"request message socket error {result.State} {result.StateMessage}");
                        _state.Value = ConnectionState.SocketError;
                        break;
                }
            }
        }

        private void SuccessHandler(ReadMessageResult result)
        {
            IMessage message = result.Bytes.ToMessage();
            result.Dispose();
            OnReceiveMessage?.Invoke(message);
        }
    }
}
