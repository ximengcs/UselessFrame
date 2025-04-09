using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UselessFrame.Runtime.Configs;

namespace UselessFrame.Runtime
{
    public static class FrameManager
    {
        private static int _idCount;
        private static Dictionary<int, IFrameCore> _cores = new Dictionary<int, IFrameCore>();

        public static IFrameCore Create(FrameConfig config)
        {
            IFrameCore core = new FrameCore(_idCount++, config);
            _cores.Add(core.Id, core);
            return core;
        }

        public static UniTask Start(IFrameCore core)
        {
            FrameCore realCore = (FrameCore)core;
            return realCore.Start();
        }

        public static async UniTask Destroy(IFrameCore core)
        {
            FrameCore realCore = (FrameCore)core;
            int id = realCore.Id;
            await realCore.Destroy();
            _cores.Remove(id);
        }
    }
}
