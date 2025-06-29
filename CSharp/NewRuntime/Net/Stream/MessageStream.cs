
using System;
using System.Collections.Generic;

namespace UselessFrame.Net
{
    internal partial class Connection
    {
        internal partial class MessageStream
        {
            private Connection _connection;
            private ConnectionSetting _setting;
            private Dictionary<Guid, WaitResponseHandle> _waitResponseList;

            public MessageStream(Connection connection)
            {
                _connection = connection;
                _setting = connection.GetRuntimeData<ConnectionSetting>();
                _waitResponseList = new Dictionary<Guid, WaitResponseHandle>();
            }

            public void CancelAllWait()
            {
                foreach (var entry in _waitResponseList)
                {
                    entry.Value.SetCancel();
                }
                _waitResponseList.Clear();
            }

            public void Dispose()
            {
                if (_waitResponseList.Count > 0)
                    CancelAllWait();
                _connection = null;
                _waitResponseList = null;
            }
        }
    }
}
