
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Net;
using UselessFrame.Runtime.Observable;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        long Id { get; }

        IPEndPoint Host { get; }

        ISubject<IServer, ServerState> State { get; }

        event Action<IConnection> NewConnectionEvent;

        IReadOnlyList<IConnection> Connections { get; }

        IConnection GetConnection(long id);

        void Start();

        void Close();

        void Broadcast(IMessage message);
    }
}
