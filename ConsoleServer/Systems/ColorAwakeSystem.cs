
using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.ECS;
using UselessFrameCommon.Entities;

namespace TestIMGUI
{
    public class ColorAwakeSystem : IAwakeSystem<ColorComponent>
    {
        public void OnAwake(ColorComponent comp)
        {
            comp.Color = comp.GetRandom().RandHSVColor();
            X.Log.Debug($" color change to {comp.Color}");
        }
    }
}
