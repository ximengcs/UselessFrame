
using System;
using System.Net;
using Google.Protobuf;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    public interface IConnection
    {
        Guid Id { get; }

        IPEndPoint LocalIP { get; }

        IPEndPoint RemoteIP { get; }

        event Action<MessageResult> ReceiveMessageEvent;

        event Action<ConnectionState, ConnectionState> StateChangeEvent;

        void Send(IMessage message);

        UniTask<MessageResult> SendWait(IMessage message);
    }
}
