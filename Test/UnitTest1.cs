
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime;
using UselessFrame.Runtime.Configs;
using UselessFrameTest.Modules;

namespace UselessFrameTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            IFrameCore core = FrameManager.Create(FrameConfig.Default);
            await core.AddModule(typeof(TestModule1), null);
            await core.Start();
            await core.Destroy();
        }
    }
}