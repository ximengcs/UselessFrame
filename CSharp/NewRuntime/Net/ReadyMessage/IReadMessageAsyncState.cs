
using System;

namespace XFrameShare.Network
{
    public interface IReadMessageAsyncState : IDisposable
    {
        public ReadMessageAsyncCallback userCallback { get; }
        public int bytesReceived { get; set; }
        public byte[] buffer { get; set; }
        public int messageSize { get; set; }
        public void Begin(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
        public int End(IAsyncResult ar);
    }
}
