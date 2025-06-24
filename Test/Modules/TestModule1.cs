
using UselessFrame.Runtime;
using UselessFrame.Runtime.Diagnotics;

namespace UselessFrameTest.Modules
{
    [Module(1)]
    public class TestModule1 : ModuleBase, IUpdater
    {
        public void OnUpdate(float detalTime)
        {

        }

        protected override void OnInit(object param)
        {
            Core.AddModule(typeof(TestModule2));
        }
    }
}
