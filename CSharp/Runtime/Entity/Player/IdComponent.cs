
using MemoryPack;

namespace UselessFrame.NewRuntime.ECS
{
    [MemoryPackable]
    public partial class IdComponent : EntityComponent
    {
        public long Id { get; internal set; }
    }
}
