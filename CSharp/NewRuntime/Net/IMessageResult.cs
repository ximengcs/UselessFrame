
using System;
using Google.Protobuf;

namespace UselessFrame.Net
{
    public interface IMessageResult
    {
        IMessage Message { get; }

        bool RequireResponse { get; }

        Type MessageType { get; }

        IConnection From { get; }
    }
}
