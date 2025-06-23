
using Google.Protobuf;

namespace UselessFrame.Net
{
    internal class NetPoolUtility
    {
        internal static NetObjectPool<ReadMessageTcpClientAsyncState> _readMessageAsyncPool;
        internal static NetObjectPool<ReadMessageResult> _readMessageResultPool;
        internal static NetObjectPool<MessageResult> _messageResultPool;

        public static void InitializePool()
        {
            _readMessageAsyncPool = new NetObjectPool<ReadMessageTcpClientAsyncState>();
            _readMessageResultPool = new NetObjectPool<ReadMessageResult>();
            _messageResultPool = new NetObjectPool<MessageResult>();
        }
    }
}
