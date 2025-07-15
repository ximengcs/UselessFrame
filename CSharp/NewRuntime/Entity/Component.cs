
using MemoryPack;

namespace UselessFrame.NewRuntime.Entities
{
    [MemoryPackable]
    public partial class Component
    {
        private Entity _entity;

        [MemoryPackIgnore]
        public Entity Entity => _entity;

        internal void OnInit(Entity entity)
        {
            _entity = entity;
        }

        internal void OnDestroy()
        {
            _entity = null;
        }
    }
}
