
using UselessFrame.Runtime;
using UselessFrame.Runtime.Diagnotics;

namespace UselessFrameTest.Modules
{
    public class TestModule2 : ModuleBase
    {
        public override void OnInit(object param)
        {
            Log.Debug($"TestModule2 OnInit 2");
        }
    }
}
