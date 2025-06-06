using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    public class MessageUtility
    {
        public readonly static byte[] CloseData = new byte[0];

        public static bool IsCloseMessage(byte[] message)
        {
            return message.Length == 0;
        }

        public static MessageWriteBuffer RequestWriteBuffer(int msgSize, ByteBufferPool pool)
        {
            return new MessageWriteBuffer(pool.Require(msgSize + Crc16CcittKermit.CRCLength + sizeof(int)), msgSize);
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
