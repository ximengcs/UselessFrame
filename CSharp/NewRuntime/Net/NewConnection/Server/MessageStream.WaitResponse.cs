
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using TestIMGUI.Core;
using UselessFrame.Net;
using static UselessFrame.Net.NetUtility;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal partial class MessageStream
        {
            public struct WaitResponseHandle
            {
                private AutoResetUniTaskCompletionSource<ReadMessageResult> _responseTaskSource;

                public readonly Guid Id;
                public readonly bool HasResponse;

                public UniTask<ReadMessageResult> ResponseTask => _responseTaskSource.Task;

                public WaitResponseHandle(IMessage requestMessage)
                {
                    Id = Guid.NewGuid();
                    HasResponse = true;
                    MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(requestMessage);
                    typeInfo.SetRequestToken(requestMessage, Id);
                    _responseTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create();
                }

                public void SetResponse(ReadMessageResult messageResult)
                {
                    _responseTaskSource.TrySetResult(messageResult);
                }

                public void SetCancel()
                {
                    _responseTaskSource.TrySetResult(new ReadMessageResult(NetOperateState.Cancel, "cancel"));
                }

            }
        }
    }

}
