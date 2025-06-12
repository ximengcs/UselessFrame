
using System;
using System.Collections.Generic;

namespace NewConnection
{
    internal partial class ServerConnection
    {
        internal partial class MessageStream
        {
            private ServerConnection _connection;
            private Dictionary<Guid, WaitResponseHandle> _waitResponseList;

            public MessageStream(ServerConnection connection)
            {
                _connection = connection;
                _waitResponseList = new Dictionary<Guid, WaitResponseHandle>();
            }

            public void CancelAllWait()
            {
                foreach (var entry in _waitResponseList)
                {
                    entry.Value.Dispose();
                }
            }

        }
    }
}
