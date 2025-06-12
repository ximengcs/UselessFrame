
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using UselessFrame.Net;
using static UselessFrame.Net.NetUtility;

namespace NewConnection
{
    internal partial class Sender
    {
        private struct WaitResponseHandle
        {
            private AutoResetUniTaskCompletionSource<ReadMessageResult> _responseTaskSource;

            public readonly Guid Id;

            public UniTask<ReadMessageResult> ResponseTask => _responseTaskSource.Task;

            public WaitResponseHandle(IMessage message)
            {
                Id = Guid.NewGuid();
                MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
                typeInfo.SetRequestToken(message, Id);
                _responseTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create();
            }

            public void SetResponse(IMessage message)
            {
                _responseTaskSource.TrySetResult(message);
            }

            public void Dispose()
            {
                _responseTaskSource.TrySetCanceled();
            }
        }
    }
}
