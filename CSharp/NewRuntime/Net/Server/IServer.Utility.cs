
using System.Net;

namespace UselessFrame.Net
{
    public partial interface IServer
    {
        public static IServer Create(int port)
        {
            return new Server(port);
        }

        public static IConnection Connect(IPEndPoint ip)
        {
            return new Connection(ip);
        }
    }
}
