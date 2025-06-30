using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal class AsyncStateUtility
    {
        public static async ValueTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener, IFiber fiber)
        {
            WaitConnectTcpClientAsyncState state = new WaitConnectTcpClientAsyncState(listener, fiber);
            return await state.CompleteTask;
        }

        public static async ValueTask<RequestConnectResult> RequestConnectAsync(TcpClient remote, IPEndPoint ipEndPoint, IFiber fiber)
        {
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(remote, ipEndPoint, fiber);
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

        public static async ValueTask<ReadMessageResult> ReadMessageAsync(TcpClient client, ByteBufferPool pool, IFiber fiber)
        {
            ReadMessageTcpClientAsyncState state = new ReadMessageTcpClientAsyncState(client, pool, fiber);
            return await state.CompleteTask;
        }

        internal static void RunToFiber<T>(object data)
        {
            var tuple = (Tuple<AutoResetUniTaskCompletionSource<T>, T>)data;
            tuple.Item1.TrySetResult(tuple.Item2);
        }

        internal static void RunToFiber(object data)
        {
            var taskSource = (AutoResetUniTaskCompletionSource)data;
            taskSource.TrySetResult();
        }
    }
}
