
using Cysharp.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UselessFrame.NewRuntime.Net.Conection;

namespace UselessFrame.Net
{
    public class ConnectionUtility
    {
        public static IPEndPoint GetLocalIPEndPoint(int port, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            IPAddress ip = GetLocalIPAddress(addressFamily);
            return new IPEndPoint(ip, port);
        }

        public static IPAddress GetLocalIPAddress(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = null;
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == addressFamily)
                {
                    ipAddress = address;
                    break;
                }
            }
            return ipAddress;
        }

        public static async UniTask<AcceptConnectResult> AcceptConnectAsync(TcpListener listener, CancellationToken cancelToken = default)
        {
            WaitConnectTcpClientAsyncState state = new WaitConnectTcpClientAsyncState(listener, cancelToken);
            return await state.CompleteTask;
        }

        public static async UniTask<RequestConnectResult> RequestConnectAsync(IPEndPoint ipEndPoint, CancellationToken cancelToken = default)
        {
            RequestConnectTcpClientAsyncState state = new RequestConnectTcpClientAsyncState(ipEndPoint, cancelToken);
            return await state.CompleteTask;
        }
    }
}
