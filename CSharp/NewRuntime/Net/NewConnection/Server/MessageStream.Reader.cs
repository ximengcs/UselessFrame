
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
                _reading = true;
                RequestMessage().Forget();
            }

            public void StopRead()
            {
                _reading = false;
            }

            private async UniTask RequestMessage()
            {
                ReadMessageResult result = await MessageUtility.ReadMessageAsync(_connection._client, _connection._pool);
                IMessage message = result.Message;
                MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
                if (typeInfo.HasResponseToken)
                {
                    if (_waitResponseList.Remove(typeInfo.GetResponseToken(message), out WaitResponseHandle handle))
                    {
                        handle.SetResponse(result);
                    }
                    else
                    {
                        X.SystemLog.Debug($"waitResponseList TryRemove ERROR");
                    }
                }

                _connection._fsm.Current.OnReceiveMessage(result);
                if (_reading)
                {
                    RequestMessage().Forget();
                }
            }
        }
    }

}
