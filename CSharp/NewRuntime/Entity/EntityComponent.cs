
using MemoryPack;

namespace UselessFrame.NewRuntime.Entities
{
    [MemoryPackable]
    public partial class EntityComponent
    {
        private Entity _entity;

        [MemoryPackIgnore]
        public Entity Entity => _entity;

        internal void OnInit(Entity entity)
        {
            _entity = entity;
            OnInit();
        }

        protected virtual void OnInit() { }

        internal void OnDestroy()
        {
            OnDispose();
            _entity = null;
        }

        protected virtual void OnDispose()
        {

        }
    }
}
