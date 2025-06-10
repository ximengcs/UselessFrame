using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal struct ReadMessageTcpClientAsyncState
    {
        private int _bytesReceived;
        private int _messageSize;
        private byte[] _buffer;
        private NetworkStream _stream;
        private ByteBufferPool _bufferPool;
        private CancellationToken _cancelToken;
        private AutoResetUniTaskCompletionSource<ReadMessageResult> _completeTaskSource;

        public UniTask<ReadMessageResult> CompleteTask => _completeTaskSource.Task;

        public ReadMessageTcpClientAsyncState(TcpClient socket, ByteBufferPool pool, CancellationToken cancelToken)
        {
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
                Complete(new ReadMessageResult(NetOperateState.SocketError, $"[Net]read message begin socket error exception:{e}"));
                return;
            }
            catch (InvalidOperationException e)
            {
                Complete(new ReadMessageResult(NetOperateState.SocketError, $"[Net]read message begin socket error exception:{e}"));
                return;
            }
            _cancelToken = cancelToken;
            Begin(0, _buffer.Length);
        }

        private void Complete(ReadMessageResult result)
        {
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
                Complete(new ReadMessageResult(NetOperateState.ParamError, $"[Net]read message begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new ReadMessageResult(NetOperateState.InValidRequest, $"[Net]read message begin param error, buffer length {_buffer.Length}, offset {offset}, size {size}, bytesReceived {_bytesReceived}, messageSize{_messageSize}, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new ReadMessageResult(NetOperateState.SocketError, $"[Net]read message begin socket error exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new ReadMessageResult(NetOperateState.Disconnect, $"[Net]read message begin stream closing, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    Complete(new ReadMessageResult(NetOperateState.SocketError, $"[Net]read message begin io socket error exception:{e}"));
                }
                else
                {
                    Complete(new ReadMessageResult(NetOperateState.Unknown, $"[Net]read message begin io error exception:{e}"));
                }
            }
        }

        public void OnReceive(IAsyncResult ar)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                Complete(new ReadMessageResult(NetOperateState.Cancel, "[Net]read messge cancel."));
                return;
            }

            int count = 0;
            try
            {
                count = _stream.EndRead(ar);
            }
            catch (ObjectDisposedException e)
            {
                Complete(new ReadMessageResult(NetOperateState.Disconnect, $"[Net]read message begin stream closing, exception:{e}"));
                return;
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    Complete(new ReadMessageResult(NetOperateState.SocketError, $"[Net]read message begin socket error exception:{e}"));
                }
                else
                {
                    Complete(new ReadMessageResult(NetOperateState.Unknown, $"[Net]read message begin io error exception:{e}"));
                }
                return;
            }

            _bytesReceived += count;
            // reading the size of the data 
            if (_messageSize == -1)
            {
                if (count == 0)
                {
                    Complete(new ReadMessageResult(NetOperateState.RemoteClose, "[Net]The remote peer closed the connection while reading the message size."));
                    return;
                }

                if (_bytesReceived == 4)//we have received the entire message size information
                {
                    //read the size of the message
                    _messageSize = BitConverter.ToInt32(_buffer, 0);
                    //Console.WriteLine($"[Net] read size success -> {_messageSize}");
                    if (_messageSize < 0)
                    {
                        Complete(new ReadMessageResult(NetOperateState.DataError, "[Net]The remote peer sent a negative message size."));
                        return;
                    }

                    //we should do some size validation here also (e.g. restrict incoming messages to x bytes long)
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
                    Complete(new ReadMessageResult(null, _messageSize, _bufferPool, NetOperateState.NormalClose));
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
                        Complete(new ReadMessageResult(NetOperateState.DataError, $"[Net]socket receive data bit eror, buffer size {_buffer.Length}, message size {_messageSize}, source crc {src}, current crc {cur}"));
                    }
                }
                else //need more data.
                {
                    if (count == 0)
                    {
                        Complete(new ReadMessageResult(NetOperateState.RemoteClose, "[Net]The remote peer closed the connection before the entire message was received."));
                        return;
                    }
                    Begin(_bytesReceived, _messageSize - _bytesReceived);
                }
            }
        }
    }
}
