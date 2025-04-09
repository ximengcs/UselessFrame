
using UselessFrame.Runtime;
using UselessFrame.Runtime.Diagnotics;

namespace UselessFrameTest.Modules
{
    public class TestModule1 : ModuleBase, IUpdater
    {
        public void OnUpdate(float detalTime)
        {
            Console.WriteLine("update1");
        }

        protected override void OnInit(object param)
        {
            Log.Debug($"TestModule1 OnInit 2");
            Core.AddModule(typeof(TestModule2), null);
        }
    }
}
