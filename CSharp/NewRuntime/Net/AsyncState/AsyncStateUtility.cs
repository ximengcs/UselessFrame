using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal class AsyncStateUtility
    {
        public static async ValueTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener)
        {
            WaitConnectTcpClientAsyncState state = new WaitConnectTcpClientAsyncState(listener);
            return await state.CompleteTask;
        }

        public static async ValueTask<RequestConnectResult> RequestConnectAsync(TcpClient remote, IPEndPoint ipEndPoint)
        {
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(remote, ipEndPoint);
            return await state.CompleteTask;
        }

        public static async ValueTask<WriteMessageResult> WriteCloseMessageAsync(TcpClient client)
        {
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, MessageWriteBuffer.CloseBuffer);
            return await state.CompleteTask;
        }

        public static async ValueTask<WriteMessageResult> WriteMessageAsync(TcpClient client, MessageWriteBuffer buffer)
        {
            ushort crc = Crc16CcittKermit.ComputeChecksum(buffer.Message);
            BitConverter.TryWriteBytes(buffer.CrcHead, crc);
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, buffer);
            return await state.CompleteTask;
        }

        public static async ValueTask<ReadMessageResult> ReadMessageAsync(TcpClient client, ByteBufferPool pool)
        {
            ReadMessageTcpClientAsyncState state = new ReadMessageTcpClientAsyncState(client, pool);
            return await state.CompleteTask;
        }
    }
}
