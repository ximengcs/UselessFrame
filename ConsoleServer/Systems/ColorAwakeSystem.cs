
using TestIMGUI.Entities;
using UselessFrame.NewRuntime.ECS;

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
