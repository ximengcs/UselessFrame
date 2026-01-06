
using MemoryPack;
using System.Buffers;
using UselessFrame.NewRuntime.ECS;

namespace TestServer
{
    public class Perch
    {
        public static void Do()
        {
            TestIMGUI.Entities.Perch.Do();
            UselessFrame.NewRuntime.Perch.Do();
        }
    }
}
