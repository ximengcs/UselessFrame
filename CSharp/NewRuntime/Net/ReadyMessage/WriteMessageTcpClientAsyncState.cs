using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal struct WriteMessageTcpClientAsyncState
    {
        private NetworkStream _stream;
        private MessageWriteBuffer _buffer;
        private AutoResetUniTaskCompletionSource<WriteMessageResult> _completeTaskSource;

        public UniTask<WriteMessageResult> CompleteTask => _completeTaskSource.Task;

        public WriteMessageTcpClientAsyncState(TcpClient client, MessageWriteBuffer buffer)
        {
            _stream = client.GetStream();
            _buffer = buffer;
            _completeTaskSource = AutoResetUniTaskCompletionSource<WriteMessageResult>.Create();
            Begin(0, buffer.PackageSize);
        }

        private void Complete(WriteMessageResult result)
        {
            _completeTaskSource.TrySetResult(result);
        }

        private void Begin(int offset, int size)
        {
            _stream.BeginWrite(_buffer.Package, offset, size, OnWrite, null);
        }

        private void OnWrite(IAsyncResult ar)
        {
            try
            {
                _stream.EndWrite(ar);
                Complete(new WriteMessageResult(NetMessageState.OK));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
