using System;
using System.Net.Sockets;

namespace XFrameShare.Network
{
    internal class ReadMessageAsyncState : IDisposable, IReadMessageAsyncState
    {
        public Socket userSocket { get; private set; }
        public ReadMessageAsyncCallback userCallback { get; private set; }
        public int bytesReceived { get; set; }
        public byte[] buffer { get; set; }
        public int messageSize { get; set; }

        public ReadMessageAsyncState(Socket socket, ReadMessageAsyncCallback callback)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");

            if (callback == null)
                throw new ArgumentNullException("callback");

            userSocket = socket;
            userCallback = callback;
            buffer = new byte[4];
            bytesReceived = 0;
            messageSize = -1;
        }

        public void Dispose()
        {
            userSocket = null;
            userCallback = null;
            buffer = null;
        }

        public void Begin(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            userSocket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
        }

        public int End(IAsyncResult ar)
        {
            return userSocket.EndReceive(ar);
        }
    }
}
