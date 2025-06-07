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
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, MessageWriteBuffer.CloseBuffer, CancellationToken.None);
            return await state.CompleteTask;
        }

        public static async UniTask<WriteMessageResult> WriteMessageAsync(TcpClient client, MessageWriteBuffer buffer, CancellationToken cancelToken = default)
        {
            ushort crc = Crc16CcittKermit.ComputeChecksum(buffer.Message);
            BitConverter.TryWriteBytes(buffer.CrcHead, crc);
            WriteMessageTcpClientAsyncState state = new WriteMessageTcpClientAsyncState(client, buffer, cancelToken);
            return await state.CompleteTask;
        }

        public static async UniTask<ReadMessageResult> ReadMessageAsync(TcpClient client, ByteBufferPool pool, CancellationToken cancelToken = default)
        {
            ReadMessageTcpClientAsyncState state = new ReadMessageTcpClientAsyncState(client, pool, cancelToken);
            return await state.CompleteTask;
        }
    }
}
