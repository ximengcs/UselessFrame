﻿
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

        event Action<IMessageResult> ReceiveMessageEvent;

        UniTask Send(IMessage message);

        UniTask<IMessageResult> SendWait(IMessage message);

        void Close();
    }
}
