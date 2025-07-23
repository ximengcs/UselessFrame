using UselessFrame.NewRuntime;
using UselessFrame.Runtime;
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