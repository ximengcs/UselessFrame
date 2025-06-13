
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        event Action<ServerState, ServerState> StateChangeEvent;

        event Action<IConnection> NewConnectionEvent;

        IReadOnlyList<IConnection> Connections { get; }

        void RemoveConnection(IConnection connection);

        void Start();

        void Close();
    }
}
