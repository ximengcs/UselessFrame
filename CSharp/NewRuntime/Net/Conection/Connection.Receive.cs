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
            switch (_state.Value)
            {
                case ConnectionState.TokenPending:
                case ConnectionState.Normal:
                    switch (result.State)
                    {
                        case NetOperateState.OK:
                            SuccessHandler(result);
                            RequestMessage().Forget();
                            break;

                        case NetOperateState.InValidRequest:
                            X.SystemLog.Debug("Net", $" {Id} reqeust message happend invalid {result.State} {result.StateMessage}");
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
                            X.SystemLog.Debug("Net", $" {Id} request message error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.FatalErrorClose;
                            _closeTokenSource.Cancel();
                            Dispose();
                            break;

                        case NetOperateState.SocketError:
                            X.SystemLog.Debug("Net", $" {Id} request message socket error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.SocketError;
                            break;
                    }
                    break;
            }
        }

        private void SuccessHandler(ReadMessageResult result)
        {
            IMessage message = result.Bytes.ToMessage();
            result.Dispose();
            PostMessage(message);
        }

        private void PostMessage(IMessage message)
        {
            MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
            if (typeInfo.HasResponseToken)
            {
                if (_waitResponseList.TryRemove(typeInfo.GetResponseToken(message), out WaitResponseHandle handle))
                {
                    handle.SetResponse(message);
                }
                else
                {
                    X.SystemLog.Debug("waitResponseList TryRemove ERROR");
                }
                return;
            }

            if (_state.Value != ConnectionState.Normal)
                return;

            if (OnReceiveMessage == null)
                return;
            _dataFiber.Post(TriggerMessage, message);
        }

        private void TriggerMessage(object state)
        {
            MessageResult result = new MessageResult((IMessage)state, this);
            OnReceiveMessage.Invoke(result);
        }

    }
}
