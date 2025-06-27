
using System;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.Net
{
    internal struct WaitConnectTcpClientAsyncState
    {
        private TcpListener _listener;
        private IFiber _fiber;
        private AutoResetUniTaskCompletionSource<AcceptConnectResult> _completeTaskSource;

        public UniTask<AcceptConnectResult> CompleteTask => _completeTaskSource.Task;

        public WaitConnectTcpClientAsyncState(TcpListener listener, IFiber fiber)
        {
            _fiber = fiber;
            _listener = listener;
            _completeTaskSource = AutoResetUniTaskCompletionSource<AcceptConnectResult>.Create();
            Begin();
        }

        private void Complete(AcceptConnectResult result)
        {
            _fiber.Post(AsyncStateUtility.RunToFiber<AcceptConnectResult>, Tuple.Create(_completeTaskSource, result));
        }

        private void Begin()
        {
            try
            {
                _listener.BeginAcceptTcpClient(OnAccept, null);
            }
            catch (ObjectDisposedException e)
            {
                Complete(AcceptConnectResult.Create(NetOperateState.FatalError, $"[Net]accept connect begin stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(AcceptConnectResult.Create(e, $"[Net]accept connect begin socket error exception:"));
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(ar);
                Complete(AcceptConnectResult.Create(client, NetOperateState.OK));
            }
            catch (ObjectDisposedException e)
            {
                Complete(AcceptConnectResult.Create(NetOperateState.FatalError, $"[Net]accept connect end stream closing, exception:{e}"));
            }
            catch (SocketException e)
            {
                Complete(AcceptConnectResult.Create(e, $"[Net]accept connect end socket error"));
            }
        }
    }
}
