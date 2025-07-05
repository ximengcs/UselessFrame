
using System;
using System.Net;
using Google.Protobuf;
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    public interface IConnection
    {
        Guid Id { get; }

        IPEndPoint LocalIP { get; }

        IPEndPoint RemoteIP { get; }

        ISubject<IConnection, ConnectionState> State { get; }

        event Action<MessageResult> ReceiveMessageEvent;

        T GetRuntimeData<T>();

        UniTask Send(IMessage message, bool autoRelease = true);

        UniTask<MessageResult> SendWait(IMessage message, bool autoRelease = true);

        UniTask<LatencyResult> TestLatency();

        void Close();
    }
}
