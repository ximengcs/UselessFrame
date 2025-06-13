using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    public class MessageUtility
    {
        public static async UniTask<WriteMessageResult> WriteCloseMessageAsync(TcpClient client)
        {
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, MessageWriteBuffer.CloseBuffer);
            return await state.CompleteTask;
        }

        public static async UniTask<WriteMessageResult> WriteMessageAsync(TcpClient client, MessageWriteBuffer buffer)
        {
            ushort crc = Crc16CcittKermit.ComputeChecksum(buffer.Message);
            BitConverter.TryWriteBytes(buffer.CrcHead, crc);
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, buffer);
            return await state.CompleteTask;
        }

        public static async UniTask<ReadMessageResult> ReadMessageAsync(TcpClient client, ByteBufferPool pool)
        {
            ReadMessageTcpClientAsyncState state = new ReadMessageTcpClientAsyncState(client, pool);
            return await state.CompleteTask;
        }
    }
}
