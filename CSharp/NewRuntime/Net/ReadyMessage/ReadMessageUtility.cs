using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace XFrameShare.Network
{
    public class ReadMessageUtility
    {
        public const int HeadDataLength = Crc16CcittKermit.CRCLength + sizeof(int);

        /// <summary>
        /// Wraps a message. The wrapped message is ready to send to a stream.
        /// </summary>
        /// <remarks>
        /// <para>Generates a length prefix for the message and returns the combined length prefix and message.</para>
        /// </remarks>
        /// <param name="message">The message to send.</param>
        public static void WrapMessage2(byte[] message, int contentLength)
        {
            Crc16CcittKermit.ComputeChecksumBytes(message, HeadDataLength, contentLength, sizeof(int));
            BitConverter.TryWriteBytes(new Span<byte>(message, 0, sizeof(int)), contentLength + Crc16CcittKermit.CRCLength);
        }


        /*
        /// <summary>
        /// Wraps a keepalive (0-length) message. The wrapped message is ready to send to a stream.
        /// </summary>
        public static byte[] WrapKeepaliveMessage(int id, int time)
        {
            KeepaliveRequest data = new KeepaliveRequest() { Id = id, Time = time };
            byte[] byteData = Entry.GetModule<MessageModule>().Serialize(new TransitionData(-1, -1, data)).ToArray();
            return WrapMessage(byteData);
        }

        public static byte[] WrapKeepaliveConfirmMessage(int id, int time)
        {
            KeepaliveResponse data = new KeepaliveResponse() { Id = id, Time = time };
            byte[] byteData = Entry.GetModule<MessageModule>().Serialize(new TransitionData(-1, -1, data)).ToArray();
            return WrapMessage(byteData);
        }*/

        private static byte[] s_CloseData = new byte[0];

        public static bool IsCloseMessage(byte[] message)
        {
            return message.Length == 0;
        }

        public static async Task<byte[]> ReadMessageAsync(TcpClient client)
        {
            TaskCompletionSource<byte[]> source = new TaskCompletionSource<byte[]>();
            ReadMessageAsync(client, (e) => source.TrySetResult(e.MessageData));
            return await source.Task;
        }

        public static void ReadMessageAsync(TcpClient client, ReadMessageAsyncCallback callback)
        {
            ReadMessageWithTcpClientAsyncState state = new ReadMessageWithTcpClientAsyncState(client, callback);
            state.Begin(state.buffer, 0, //offset
                state.buffer.Length, new AsyncCallback(OnReceive), state);
        }


        public static void ReadMessageAsync(Socket socket, ReadMessageAsyncCallback callback)
        {
            ReadMessageAsyncState state = new ReadMessageAsyncState(socket, callback);
            state.Begin(state.buffer, 0, //offset
                state.buffer.Length, //how much data can be read
                new AsyncCallback(OnReceive), state);
        }

        private static void OnReceive(IAsyncResult ar)
        {
            IReadMessageAsyncState state = ar.AsyncState as IReadMessageAsyncState;
            try
            {
                int count = state.End(ar);
                state.bytesReceived += count;
                Console.WriteLine($"server on receive {state.buffer.Length} {count} {state.bytesReceived} {state.messageSize} {state.GetHashCode()}");
                if (state.messageSize == -1)//we are still reading the size of the data
                {
                    if (count == 0)
                        throw new ProtocolViolationException("The remote peer closed the connection while reading the message size.");

                    if (state.bytesReceived == 4)//we have received the entire message size information
                    {
                        //read the size of the message
                        state.messageSize = BitConverter.ToInt32(state.buffer, 0);
                        if (state.messageSize < 0)
                        {
                            throw new ProtocolViolationException("The remote peer sent a negative message size.");
                        }
                        //we should do some size validation here also (e.g. restrict incoming messages to x bytes long)
                        state.buffer = new Byte[state.messageSize];
                        //reset the bytes received back to zero
                        //because we are now switching to reading the message body
                        state.bytesReceived = 0;
                    }

                    if (state.messageSize != 0)
                    {
                        //we need more data - could be more of the message size information
                        //or it could be the message body. The only time we won't need to
                        //read more data is if the message size == 0
                        state.Begin(state.buffer,
                        state.bytesReceived, //offset where data can be written
                        state.buffer.Length - state.bytesReceived, //how much data can be read into remaining buffer
                        new AsyncCallback(OnReceive), state);
                    }
                    else
                    {
                        //we have received a zero length message, notify the user...
                        ReadMessageEventArgs args = new ReadMessageEventArgs(s_CloseData);
                        state.userCallback(args);
                        state.Dispose();
                    }
                }
                else //we are reading the body of the message
                {
                    if (state.bytesReceived == state.messageSize) //we have the entire message
                    {
                        if (Crc16CcittKermit.Check(state.buffer, out ushort src, out ushort cur))
                        {
                            Console.WriteLine($"server on receive Check CRC {src}");
                            ReadMessageEventArgs args = new ReadMessageEventArgs(state.buffer);
                            //free up our reference to the socket, buffer and the callback object.
                            state.userCallback(args);
                            state.Dispose();
                        }
                        else
                        {
                            Console.WriteLine(BitConverter.ToString(state.buffer));
                            Console.WriteLine($"socket receive data bit eror, buffer size {state.buffer.Length}, source crc {src}, current crc {cur}");
                            Exception ex = new Exception($"socket receive data bit eror, buffer size {state.buffer.Length}, source crc {src}, current crc {cur}");
                            ReadMessageEventArgs args = new ReadMessageEventArgs(ex);
                            state.userCallback(args);
                            state.Dispose();
                        }
                    }
                    else //need more data.
                    {
                        if (count == 0)
                            throw new ProtocolViolationException("The remote peer closed the connection before the entire message was received");
                        state.Begin(state.buffer,
                        state.bytesReceived, //offset where data can be written
                        state.buffer.Length - state.bytesReceived, //how much data can be read into remaining buffer
                        new AsyncCallback(OnReceive), state);
                    }
                }
            }
            catch (Exception ex)
            {
                ReadMessageEventArgs args = new ReadMessageEventArgs(ex);
                state.userCallback(args);
                state.Dispose();
            }
        }
    }
}
