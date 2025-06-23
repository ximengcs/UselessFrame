using System;

namespace UselessFrame.Net
{
    public struct MessageResultHandle : IDisposable
    {
        public readonly IMessageResult Result;

        public MessageResultHandle(IMessageResult result)
        {
            Result = result;
        }

        public void Dispose()
        {
            MessageResult result = (MessageResult)Result;
            result.Dispose();
        }
    }
}
