
using MemoryPack;
using Unity.Mathematics;
using UselessFrame.NewRuntime.ECS;

namespace UselessFrameCommon.Entities
{
    [MemoryPackable]
    public partial class ColorComponent : EntityComponent
    {
        public int4 Color;
    }
}
