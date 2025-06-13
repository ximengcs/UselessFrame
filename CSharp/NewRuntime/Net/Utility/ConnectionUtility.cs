
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;

namespace UselessFrame.Net
{
    internal class ConnectionUtility
    {
        public static async UniTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener)
        {
            WaitConnectTcpClientAsyncState state = new WaitConnectTcpClientAsyncState(listener);
            return await state.CompleteTask;
        }

        public static async UniTask<RequestConnectResult> RequestConnectAsync(TcpClient remote, IPEndPoint ipEndPoint, CancellationToken cancelToken = default)
        {
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(remote, ipEndPoint, cancelToken);
            return await state.CompleteTask;
        }
    }
}
