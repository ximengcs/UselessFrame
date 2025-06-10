
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using System;
using UselessFrame.Net;
using static UselessFrame.Net.NetUtility;

namespace TestIMGUI.Core
{
    public partial class Connection
    {
        private struct WaitResponseHandle
        {
            private AutoResetUniTaskCompletionSource<IMessage> _responseTaskSource;

            public readonly Guid Id;

            public UniTask<IMessage> ResponseTask => _responseTaskSource.Task;

            public WaitResponseHandle(IMessage message)
            {
                Id = Guid.NewGuid();
                MessageTypeInfo typeInfo = NetUtility.GetMessageTypeInfo(message);
                typeInfo.SetRequestToken(message, Id);
                _responseTaskSource = AutoResetUniTaskCompletionSource<IMessage>.Create();
            }

            public void SetResponse(IMessage message)
            {
                _responseTaskSource.TrySetResult(message);
            }
        }
    }
}
