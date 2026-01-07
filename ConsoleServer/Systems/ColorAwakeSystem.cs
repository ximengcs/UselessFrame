
using UselessFrame.NewRuntime.ECS;
using UselessFrameCommon.Entities;

namespace TestIMGUI
{
    public class ColorAwakeSystem : IAwakeSystem<ColorComponent>
    {
        public void OnAwake(ColorComponent comp)
        {
            comp.Color = comp.GetRandom().RandHSVColor();
            Console.WriteLine($"color {comp.Color}");
        }
    }
}
