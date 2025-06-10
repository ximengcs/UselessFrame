using Google.Protobuf;
using UselessFrame.Net;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime;
using static UselessFrame.Net.NetUtility;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private int _requestMessageInvalidRetryTimes;

        private async UniTaskVoid RequestMessage()
        {
            ReadMessageResult result = await MessageUtility.ReadMessageAsync(_client, _pool, _closeTokenSource.Token);
            if (_closeTokenSource.IsCancellationRequested)
                return;

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
                            if (_requestMessageInvalidRetryTimes > 0)
                            {
                                X.SystemLog.Debug("Net", $" {Id} will retry request message ({_requestMessageInvalidRetryTimes})");
                                _requestMessageInvalidRetryTimes--;
                                RequestMessage().Forget();
                            }
                            else
                            {
                                X.SystemLog.Debug("Net", $" {Id} will retry times is ({_requestMessageInvalidRetryTimes}), will close");
                                _state.Value = ConnectionState.FatalErrorClose;
                                InnerClose();
                            }
                            break;

                        case NetOperateState.NormalClose:
                            X.SystemLog.Debug("Net", $" {Id} will normal close ");
                            _state.Value = ConnectionState.NormalClose;
                            InnerClose();
                            break;

                        case NetOperateState.Disconnect:
                        case NetOperateState.DataError:
                        case NetOperateState.ParamError:
                        case NetOperateState.PermissionError:
                        case NetOperateState.RemoteClose:
                        case NetOperateState.Unknown:
                            X.SystemLog.Debug("Net", $" {Id} request message fatal error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.FatalErrorClose;
                            InnerClose();
                            break;

                        case NetOperateState.SocketError:
                            X.SystemLog.Debug("Net", $" {Id} request message socket error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.SocketError;
                            InnerClose();
                            break;

                        default:
                            X.SystemLog.Debug("Net", $" {Id} request message unkown error {result.State} {result.StateMessage}");
                            _state.Value = ConnectionState.FatalErrorClose;
                            InnerClose();
                            break;
                    }
                    break;

                default:
                    X.SystemLog.Debug("Net", $" {Id} reqeust message in error state {_state.Value}, {result.State} {result.StateMessage}");
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
                    X.SystemLog.Debug($"{Id} waitResponseList TryRemove ERROR");
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
            if (_closeTokenSource.IsCancellationRequested)
                return;

            MessageResult result = new MessageResult((IMessage)state, this);
            OnReceiveMessage.Invoke(result);
        }

    }
}
