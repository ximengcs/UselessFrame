
using Cysharp.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UselessFrame.NewRuntime.Net.Conection;

namespace UselessFrame.Net
{
    internal class ConnectionUtility
    {
        public static async UniTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener, CancellationToken cancelToken = default)
        {
            WaitConnectTcpClientAsyncState state = new WaitConnectTcpClientAsyncState(listener, cancelToken);
            return await state.CompleteTask;
        }

        public static async UniTask<RequestConnectResult> RequestConnectAsync(TcpClient remote, IPEndPoint ipEndPoint, CancellationToken cancelToken = default)
        {
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(remote, ipEndPoint, cancelToken);
            return await state.CompleteTask;
        }

        public static async UniTask<RequestConnectResult> ReConnectAsync(TcpClient client, CancellationToken cancelToken = default)
        {
            if (client.Client.RemoteEndPoint == null)
                return new RequestConnectResult(NetOperateState.Unknown, "[Net]reconnect error, remote is null");

            IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(client, remoteEndPoint, cancelToken);
            return await state.CompleteTask;
        }
    }
}
