
using Cysharp.Threading.Tasks;
using UselessFrame.Net;
using TestIMGUI.Core;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal class RunState : ConnectionState
        {
            public override void OnEnter(ConnectionState preState)
            {
                base.OnEnter(preState);
                _connection._stream.StartRead();
            }

            public override async UniTask<bool> OnReceiveMessage(ReadMessageResult messageResult, MessageStream.WaitResponseHandle responseHandle)
            {
                switch (messageResult.State)
                {
                    case NetOperateState.OK:
                        {
                            if (responseHandle.HasResponse)
                            {
                                responseHandle.SetResponse(messageResult);
                            }
                            else
                            {
                                MessageResult result = new MessageResult(messageResult.Message, _connection._stream);
                                _connection._onReceiveMessage?.Invoke(result);
                            }
                            return true;
                        }

                    default:
                        {
                            return false;
                        }
                }
            }
        }
    }
}
