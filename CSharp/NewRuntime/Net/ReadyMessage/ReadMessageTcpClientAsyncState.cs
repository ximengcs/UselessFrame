using System;
using System.Net;
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
        private AutoResetUniTaskCompletionSource<ReadMessageResult> _completeTaskSource;

        public UniTask<ReadMessageResult> CompleteTask => _completeTaskSource.Task;

        public ReadMessageTcpClientAsyncState(TcpClient socket, ByteBufferPool pool)
        {
            _bufferPool = pool;
            _buffer = _bufferPool.Require(4);
            _bytesReceived = 0;
            _messageSize = -1;
            _completeTaskSource = AutoResetUniTaskCompletionSource<ReadMessageResult>.Create();
            _stream = socket.GetStream();
            Begin(0, _buffer.Length);
        }

        private void Complete(ReadMessageResult result)
        {
            _completeTaskSource.TrySetResult(result);
            _stream = null;
        }

        private void Begin(int offset, int size)
        {
            _stream.BeginRead(_buffer, offset, size, OnReceive, null);
        }

        public void OnReceive(IAsyncResult ar)
        {
            try
            {
                int count = _stream.EndRead(ar);
                _bytesReceived += count;

                // reading the size of the data 
                if (_messageSize == -1)
                {
                    if (count == 0)
                        throw new ProtocolViolationException("The remote peer closed the connection while reading the message size.");

                    if (_bytesReceived == 4)//we have received the entire message size information
                    {
                        //read the size of the message
                        _messageSize = BitConverter.ToInt32(_buffer, 0);
                        if (_messageSize < 0)
                            throw new ProtocolViolationException("The remote peer sent a negative message size.");
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
                        Begin(_bytesReceived, _buffer.Length - _bytesReceived); //how much data can be read into remaining buffer
                    }
                    else
                    {
                        //we have received a zero length message, notify the user...
                        Complete(new ReadMessageResult(null, _messageSize, _bufferPool, NetMessageState.Close));
                    }
                }
                else //we are reading the body of the message
                {
                    if (_bytesReceived == _messageSize) //we have the entire message
                    {
                        if (Crc16CcittKermit.Check(_buffer, out ushort src, out ushort cur))
                        {
                            Complete(new ReadMessageResult(_buffer, _messageSize, _bufferPool));
                        }
                        else
                        {
                            throw new ProtocolViolationException($"socket receive data bit eror, buffer size {_buffer.Length}, source crc {src}, current crc {cur}");
                        }
                    }
                    else //need more data.
                    {
                        if (count == 0)
                            throw new ProtocolViolationException("The remote peer closed the connection before the entire message was received");
                        Begin(_bytesReceived, _buffer.Length - _bytesReceived);
                    }
                }
            }
            catch (Exception ex)
            {
                Complete(new ReadMessageResult(_buffer, _messageSize, _bufferPool, ex));
            }
        }
    }
}
