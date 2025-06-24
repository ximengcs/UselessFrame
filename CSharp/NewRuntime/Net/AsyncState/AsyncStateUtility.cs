using System;
using System.Net;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal class AsyncStateUtility
    {
        public static async UniTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener, IFiber fiber)
        {
            using (WaitConnectTcpClientAsyncState state = NetPoolUtility._waitConnectAsyncPool.Require())
            {
                state.Initialize(listener, fiber);
                return await state.CompleteTask;
            }
        }

        public static async UniTask<RequestConnectResult> RequestConnectAsync(TcpClient remote, IPEndPoint ipEndPoint, IFiber fiber)
        {
            using (RequestConnectTcpClientAsyncState state = NetPoolUtility._requestConnectAsyncPool.Require())
            {
                state.Initialize(remote, ipEndPoint, fiber);
                return await state.CompleteTask;
            }
        }

        public static async UniTask<WriteMessageResult> WriteCloseMessageAsync(TcpClient client, IFiber fiber)
        {
            using (WriteMessageTcpClientAsyncState state = NetPoolUtility._writeMessageAsyncPool.Require())
            {
                state.Initialize(client, MessageWriteBuffer.CloseBuffer, fiber);
                return await state.CompleteTask;
            }
        }

        public static async UniTask<WriteMessageResult> WriteMessageAsync(TcpClient client, MessageWriteBuffer buffer, IFiber fiber)
        {
            ushort crc = Crc16CcittKermit.ComputeChecksum(buffer.Message);
            BitConverter.TryWriteBytes(buffer.CrcHead, crc);
            using (WriteMessageTcpClientAsyncState state = NetPoolUtility._writeMessageAsyncPool.Require())
            {
                state.Initialize(client, buffer, fiber);
                return await state.CompleteTask;
            }
        }

        public static async UniTask<ReadMessageResult> ReadMessageAsync(TcpClient client, ByteBufferPool pool, IFiber fiber)
        {
            using (ReadMessageTcpClientAsyncState state = NetPoolUtility._readMessageAsyncPool.Require())
            {
                state.Initialize(client, pool, fiber);
                ReadMessageResult result = await state.CompleteTask;
                return result;
            }
        }
    }
}
