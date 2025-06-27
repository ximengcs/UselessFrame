
using Google.Protobuf;
using UselessFrame.Net;
using UselessFrame.NewRuntime;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;
using System;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class MessageStream
        {
            private bool _reading;

            public void StartRead()
            {
                RequestMessage().Forget();
            }

            private async UniTask RequestMessage()
            {
                if (_reading)
                    return;

                _reading = true;
                while (_reading)
                {
                    ReadMessageResult result = await AsyncStateUtility.ReadMessageAsync(_connection._client, _connection._pool, _connection._runFiber);
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
                                handle = WaitResponseHandle.CreateEmpty();
                            }
                        }
                        else
                        {
                            handle = WaitResponseHandle.CreateEmpty();
                        }
                    }
                    else
                    {
                        handle = WaitResponseHandle.CreateEmpty();
                    }

                    _reading = await _connection._fsm.Current.TriggerReceiveMessage(result, handle);
                    await UniTask.CompletedTask;
                }
            }
        }
    }

}
