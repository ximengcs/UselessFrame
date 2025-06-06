using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace UselessFrame.Net
{
    internal struct WriteMessageTcpClientAsyncState
    {
        private NetworkStream _stream;
        private MessageWriteBuffer _buffer;
        private CancellationToken _cancelToken;
        private AutoResetUniTaskCompletionSource<WriteMessageResult> _completeTaskSource;

        public UniTask<WriteMessageResult> CompleteTask => _completeTaskSource.Task;

        public WriteMessageTcpClientAsyncState(TcpClient client, MessageWriteBuffer buffer, CancellationToken cancelToken)
        {
            _cancelToken = cancelToken;
            _stream = client.GetStream();
            _buffer = buffer;
            _completeTaskSource = AutoResetUniTaskCompletionSource<WriteMessageResult>.Create();
            Begin(0, buffer.PackageSize);
        }

        private void Complete(WriteMessageResult result)
        {
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
                Complete(new WriteMessageResult(NetMessageState.ParamError, $"[Net]write message begin param is null, exception:{e}"));
            }
            catch (ArgumentOutOfRangeException e)
            {
                Complete(new WriteMessageResult(NetMessageState.ParamError, $"[Net]write message begin param error, exception:{e}"));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new WriteMessageResult(NetMessageState.Disconnect, $"[Net]write message begin stream closing, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    Complete(new WriteMessageResult(NetMessageState.SocketError, $"[Net]write message begin socket error exception:{e}"));
                }
                else
                {
                    Complete(new WriteMessageResult(NetMessageState.Unknown, $"[Net]write message begin io error exception:{e}"));
                }
            }
        }

        private void OnWrite(IAsyncResult ar)
        {
            try
            {
                _stream.EndWrite(ar);
                Complete(new WriteMessageResult(NetMessageState.OK));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new WriteMessageResult(NetMessageState.Disconnect, $"[Net]write message begin stream closing, exception:{e}"));
            }
            catch (IOException e)
            {
                if (e.InnerException is SocketException)
                {
                    Complete(new WriteMessageResult(NetMessageState.SocketError, $"[Net]write message begin socket error exception:{e}"));
                }
                else
                {
                    Complete(new WriteMessageResult(NetMessageState.Unknown, $"[Net]write message begin io error exception:{e}"));
                }
            }
        }
    }
}
