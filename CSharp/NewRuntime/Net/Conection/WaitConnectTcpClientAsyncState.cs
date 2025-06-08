
using System;
using System.Threading;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal struct WaitConnectTcpClientAsyncState
    {
        private TcpListener _listener;
        private CancellationToken _cancelToken;
        private AutoResetUniTaskCompletionSource<AcceptConnectResult> _completeTaskSource;

        public UniTask<AcceptConnectResult> CompleteTask => _completeTaskSource.Task;

        public WaitConnectTcpClientAsyncState(TcpListener listener, CancellationToken cancelToken)
        {
            _listener = listener;
            _cancelToken = cancelToken;
            _completeTaskSource = AutoResetUniTaskCompletionSource<AcceptConnectResult>.Create();
            Begin();
        }

        private void Complete(AcceptConnectResult result)
        {
            _completeTaskSource.TrySetResult(result);
            _listener = null;
        }

        private void Begin()
        {
            try
            {
                _listener.BeginAcceptTcpClient(OnAccept, null);
            }
            catch (ObjectDisposedException e)
            {
                Complete(new AcceptConnectResult(NetOperateState.Disconnect, $"[Net]accept connect begin stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new AcceptConnectResult(NetOperateState.SocketError, $"[Net]accept connect begin socket error exception:{e}"));
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            if (_cancelToken.IsCancellationRequested)
            {
                Complete(new AcceptConnectResult(NetOperateState.Cancel, "[Net]accept connect cancel."));
                return;
            }

            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(ar);

                if (client == null || client.Client == null || !client.Connected)
                {
                    client?.Close();
                    Complete(new AcceptConnectResult(NetOperateState.InValidRequest, $"accept connect is invalid"));
                    return;
                }

                Complete(new AcceptConnectResult(client, NetOperateState.OK));
            }
            catch (ObjectDisposedException e)
            {
                Complete(new AcceptConnectResult(NetOperateState.Disconnect, $"[Net]accept connect end stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(new AcceptConnectResult(NetOperateState.SocketError, $"[Net]accept connect end socket error exception:{e}"));
            }
        }
    }
}
