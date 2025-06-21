using System;
using System.IO;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal struct ReadMessageTcpClientAsyncState
    {
        private int _bytesReceived;
        private int _messageSize;
        private byte[] _buffer;
        private int _readTimes;
        private NetworkStream _stream;
        private ByteBufferPool _bufferPool;
        private IFiber _fiber;
        private AutoResetUniTaskCompletionSource<ReadMessageResult> _completeTaskSource;

        public UniTask<ReadMessageResult> CompleteTask => _completeTaskSource.Task;

        public ReadMessageTcpClientAsyncState(TcpClient socket, ByteBufferPool pool, IFiber fiber)
        {
            _fiber = fiber;
            _readTimes = 0;
            _bufferPool = pool;
            _buffer = _bufferPool.Require(4);
            _bytesReceived = 0;
            _messageSize = -1;
            _completeTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create();
            _stream = null;
            try
            {
                _stream = socket.GetStream();
            }
            catch (ObjectDisposedException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin socket error, readTimes {_readTimes}, exception:{e}"));
                return;
            }
            catch (InvalidOperationException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin socket error, readTimes {_readTimes}, exception:{e}"));
                return;
            }
            Begin(0, _buffer.Length);
        }

        private void Complete(ReadMessageResult result)
        {
            if (_buffer != null)
            {
                _bufferPool.Release(_buffer);
                _buffer = null;
            }
            _fiber.Post(ResultToFiber, result);
        }

        private void ResultToFiber(object data)
        {
            ReadMessageResult result = (ReadMessageResult)data;
            _completeTaskSource.TrySetResult(result);
            _stream = null;
        }

        private void Begin(int offset, int size)
        {
            try
            {
                _stream.BeginRead(_buffer, offset, size, OnReceive, null);
            }
            catch (ArgumentNullException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin param is null, readTimes {_readTimes}, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin param error, readTimes {_readTimes}, buffer length {_buffer.Length}, offset {offset}, size {size}, bytesReceived {_bytesReceived}, messageSize{_messageSize}, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new ReadMessageResult(e, $"[Net]read message begin socket error, readTimes {_readTimes}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin stream closing, readTimes {_readTimes}, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException se)
                {
                    Complete(new ReadMessageResult(se, $"[Net]read message begin io socket error, readTimes {_readTimes}"));
                }
                else
                {
                    Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin io error, readTimes {_readTimes}, exception:{e}"));
                }
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            int count = 0;
            try
            {
                count = _stream.EndRead(ar);
            }
            catch (ObjectDisposedException e)
            {
                Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin stream closing, readTimes {_readTimes}, exception:{e}"));
                return;
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException socketEx)
                {
                    Complete(new ReadMessageResult(socketEx, $"[Net]read message begine socket error"));
                }
                else
                {
                    Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]read message begin io error, readTimes {_readTimes}, exception:{e}"));
                }
                return;
            }

            _bytesReceived += count;
            // reading the size of the data 
            if (_messageSize == -1)
            {
                if (count == 0)
                {
                    Complete(new ReadMessageResult(NetOperateState.RemoteClose, $"[Net]The remote peer closed the connection while reading the message size, readTimes {_readTimes},"));
                    return;
                }

                if (_bytesReceived == 4)//we have received the entire message size information
                {
                    //read the size of the message
                    _messageSize = BitConverter.ToInt32(_buffer, 0);
                    //Console.WriteLine($"[Net] read size success -> {_messageSize}");
                    if (_messageSize < 0)
                    {
                        Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]The remote peer sent a negative message size, readTimes {_readTimes},"));
                        return;
                    }

                    //we should do some size validation here also (e.g. restrict incoming messages to x bytes long)

                    if (!NetUtility.CheckMessageSize(_messageSize))
                    {
                        Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net][DENGER]The remote peer sent a large message size, messageSize is {_messageSize}. will interrupt this client"));
                        return;
                    }

                    _bufferPool.Release(_buffer);
                    _buffer = _bufferPool.Require(_messageSize);
                    //reset the bytes received back to zero
                    //because we are now switching to reading the message body
                    _bytesReceived = 0;
                }

                if (_messageSize != 0)
                {
                    //we need more data - could be more of the message size information
                    //or it could be the message body. The only time we won't need to
                    //read more data is if the message size == 0
                    Begin(_bytesReceived, _messageSize - _bytesReceived); //how much data can be read into remaining buffer
                }
                else
                {
                    //we have received a zero length message, notify the user...
                    Complete(new ReadMessageResult(null, _messageSize, _bufferPool, NetOperateState.RemoteClose));
                }
            }
            else //we are reading the body of the message
            {
                if (_bytesReceived == _messageSize) //we have the entire message
                {
                    if (Crc16CcittKermit.Check(_buffer, _messageSize, out ushort src, out ushort cur))
                    {
                        //Console.WriteLine($"[Net] read data success -> {_messageSize}");
                        Complete(new ReadMessageResult(_buffer, _messageSize, _bufferPool, NetOperateState.OK));
                    }
                    else
                    {
                        Complete(new ReadMessageResult(NetOperateState.FatalError, $"[Net]socket receive data bit eror, readTimes {_readTimes}, buffer size {_buffer.Length}, message size {_messageSize}, source crc {src}, current crc {cur}"));
                    }
                }
                else //need more data.
                {
                    if (count == 0)
                    {
                        Complete(new ReadMessageResult(NetOperateState.RemoteClose, "[Net]The remote peer closed the connection before the entire message was received., readTimes {_readTimes}"));
                        return;
                    }
                    Begin(_bytesReceived, _messageSize - _bytesReceived);
                }
            }
        }
    }
}
