
namespace UselessFrame.Net
{
    internal class NetPoolUtility
    {
        internal static NetObjectPool<ReadMessageTcpClientAsyncState> _readMessageAsyncPool;
        internal static NetObjectPool<ReadMessageResult> _readMessageResultPool;

        public static void InitializePool()
        {
            _readMessageAsyncPool = new NetObjectPool<ReadMessageTcpClientAsyncState>();
            _readMessageResultPool = new NetObjectPool<ReadMessageResult>();
        }
    }
}
