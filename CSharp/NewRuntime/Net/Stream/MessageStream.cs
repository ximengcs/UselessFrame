
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class MessageStream
        {
            private Connection _connection;
            private Dictionary<Guid, WaitResponseHandle> _waitResponseList;

            public MessageStream(Connection connection)
            {
                _connection = connection;
                _waitResponseList = new Dictionary<Guid, WaitResponseHandle>();
            }

            public void CancelAllWait()
            {
                foreach (var entry in _waitResponseList)
                {
                    entry.Value.SetCancel();
                }
            }

        }
    }
}
