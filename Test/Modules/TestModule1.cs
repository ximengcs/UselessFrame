
using UselessFrame.Runtime;

namespace UselessFrameTest.Modules
{
    [Module(1)]
    public class TestModule1 : ModuleBase, IModuleUpdater
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
