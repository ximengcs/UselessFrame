
using System;
using Google.Protobuf;
using Cysharp.Threading.Tasks;
using static UselessFrame.Net.NetUtility;

namespace UselessFrame.Net
{
    internal partial class Connection
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

                public static WaitResponseHandle CreateEmpty()
                {
                    WaitResponseHandle handle = new WaitResponseHandle()
                    {
                        _responseTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create()
                    };
                    return handle;
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
