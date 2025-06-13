
using Google.Protobuf;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal partial class MessageStream
        {
            private bool _reading;

            public void StartRead()
            {
                if (_reading)
                    return;
                _reading = true;

                RequestMessage().Forget();
            }

            private async UniTask RequestMessage()
            {
                ReadMessageResult result = await MessageUtility.ReadMessageAsync(_connection._client, _connection._pool);
                IMessage message = result.Message;
                WaitResponseHandle handle = default;
                if (message != null)
                {
                    MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
                    if (typeInfo.HasResponseToken)
                    {
                        if (!_waitResponseList.Remove(typeInfo.GetResponseToken(message), out handle))
                        {
                            X.SystemLog.Debug($"waitResponseList TryRemove ERROR");
                        }
                    }
                }

                _reading = await _connection._fsm.Current.OnReceiveMessage(result, handle);
                if (_reading)
                {
                    RequestMessage().Forget();
                }
            }
        }
    }

}
