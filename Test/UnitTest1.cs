using UselessFrame.NewRuntime;
using UselessFrame.Runtime;
using UselessFrame.Runtime.Configs;
using UselessFrame.Runtime.Extensions;
using UselessFrame.Runtime.Pools;
using UselessFrame.Runtime.Types;
using UselessFrameTest.Enums;
using UselessFrameTest.Modules;
using XFrame.Core;

namespace UselessFrameTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NewTest1()
        {
            ServerToken token = new ServerToken();

        }

        [TestMethod]
        public void NewTest2()
        {
            X.Initialize(new XSetting() { TypeFilter = new DefaultTypeFilter() });
            foreach (Type type in X.Type.GetCollection(typeof(ModuleAttribute)))
            {
                Console.WriteLine(type.FullName);
            }
        }
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

        [TestMethod]
        public void TestListPool()
        {
            List<Enum1> values = "E1,E1,E2,E1".ToEnumList<Enum1>();
            foreach (Enum1 item in values)
                Console.WriteLine($"-- {item}");

            Enum1 e1 = "E1".ToEnum<Enum1>();
            Console.WriteLine(e1);
        }
    }
}