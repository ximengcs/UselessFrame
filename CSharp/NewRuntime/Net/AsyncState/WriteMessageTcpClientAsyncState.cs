using System;
using System.IO;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal struct WriteMessageTcpClientAsyncState
    {
        private IFiber _fiber;
        private NetworkStream _stream;
        private MessageWriteBuffer _buffer;
        private AutoResetUniTaskCompletionSource<WriteMessageResult> _completeTaskSource;

        public UniTask<WriteMessageResult> CompleteTask => _completeTaskSource.Task;

        public WriteMessageTcpClientAsyncState(TcpClient client, MessageWriteBuffer buffer, IFiber fiber)
        {
            _fiber = fiber;
            _buffer = buffer;
            _completeTaskSource = AutoResetUniTaskCompletionSource<WriteMessageResult>.Create();

            _stream = null;
            try
            {
                _stream = client.GetStream();
            }
            catch (ObjectDisposedException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message ready error, exception:{e}"));
                return;
            }
            catch (InvalidOperationException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message ready error, exception:{e}"));
                return;
            }

            Begin(0, buffer.PackageSize);
        }

        private void Complete(WriteMessageResult result)
        {
            _fiber.Post(ResultToFiber, result);
        }

        private void ResultToFiber(object data)
        {
            WriteMessageResult result = (WriteMessageResult)data;
            _completeTaskSource.TrySetResult(result);
            _stream = null;
        }

        private void Begin(int offset, int size)
        {
            try
            {
                _stream.BeginWrite(_buffer.Package, offset, size, OnWrite, null);
            }
            catch (ArgumentNullException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message begin param error, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new WriteMessageResult(e, $"[Net]write message begin socket error"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message begin stream closing, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException se)
                {
                    Complete(new WriteMessageResult(se, $"[Net]write message begin io socket error"));
                }
                else
                {
                    Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message begin unkown error exception:{e}"));
                }
            }
        }

        private void OnWrite(IAsyncResult ar)
        {
            try
            {
                _stream.EndWrite(ar);
                Complete(new WriteMessageResult(NetOperateState.OK));
            }
            catch (SocketException e)
            {
                Complete(new WriteMessageResult(e, $"[Net]write message end socket error "));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message end stream closing, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException se)
                {
                    Complete(new WriteMessageResult(se, $"[Net]write message end socket error"));
                }
                else
                {
                    Complete(new WriteMessageResult(NetOperateState.FatalError, $"[Net]write message end io error exception:{e}"));
                }
            }
        }
    }
}
