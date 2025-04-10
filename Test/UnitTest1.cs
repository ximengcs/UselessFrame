using UselessFrame.Runtime;
using UselessFrame.Runtime.Configs;
using UselessFrame.Runtime.Extensions;
using UselessFrame.Runtime.Types;
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
            core.AddHandler(typeof(UpdateHandler));
            await core.AddModule(typeof(TestModule1));
            await core.Start();

            ITypeCollection types = core.TypeSystem.GetOrNew(typeof(ModuleBase));
            foreach (Type type in types)
            {
                Console.WriteLine(type.Name);
            }
            core.Trigger<IUpdater>(0.16f);
            await core.Destroy();
        }
    }
}