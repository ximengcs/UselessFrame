
using UselessFrame.Runtime;
using UselessFrame.Runtime.Diagnotics;

namespace UselessFrameTest.Modules
{
    public class TestModule1 : ModuleBase
    {
        public override void OnInit(object param)
        {
            Log.Debug($"TestModule1 OnInit 2");
            Driver.AddModule(typeof(TestModule2), null);
        }
    }
}
