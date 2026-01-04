
using MemoryPack;
using Unity.Mathematics;
using UselessFrame.NewRuntime.ECS;

namespace TestIMGUI.Entities
{
    [MemoryPackable]
    public partial class ColorComponent : EntityComponent
    {
        public int4 Color;
    }
}
