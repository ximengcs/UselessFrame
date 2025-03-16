
using Cysharp.Threading.Tasks;
using UselessFrame.Runtime;
using UselessFrameTest.Modules;

namespace UselessFrameTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            ModuleDriver driver = new ModuleDriver();
            await driver.AddModule(typeof(TestModule1), null);
            await driver.Start();
            await driver.Destroy();
        }
    }
}