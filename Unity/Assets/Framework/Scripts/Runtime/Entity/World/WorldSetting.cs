
using System.Net;
using UselessFrame.Net;
using UselessFrame.NewRuntime.Fiber;

namespace UselessFrame.NewRuntime.ECS
{
    public struct WorldSetting
    {
        public WorldType Type;
        public IPEndPoint Ip;
        public IFiber Fiber;

        public static WorldSetting Default
        {
            get => new WorldSetting()
            {
                Type = WorldType.None,
                Ip = null,
                Fiber = X.Fiber.MainFiber
            };
        }

        public static WorldSetting Server(int port)
        {
            WorldSetting setting = new WorldSetting();
            setting.Type = WorldType.Server;
            setting.Ip = NetUtility.GetLocalIPEndPoint(port);
            setting.Fiber = X.Fiber.MainFiber;
            return setting;
        }

        public static WorldSetting Client(int port)
        {
            WorldSetting setting = new WorldSetting();
            setting.Type = WorldType.Client;
            setting.Ip = NetUtility.GetLocalIPEndPoint(port);
            setting.Fiber = X.Fiber.MainFiber;
            return setting;
        }

        public static WorldSetting Client(string ip, int port)
        {
            WorldSetting setting = new WorldSetting();
            setting.Type = WorldType.Client;
            setting.Ip = new IPEndPoint(IPAddress.Parse(ip), port);
            setting.Fiber = X.Fiber.MainFiber;
            return setting;
        }
    }
}
