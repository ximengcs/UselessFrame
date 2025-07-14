
namespace UselessFrame.NewRuntime.Entities
{
    public abstract class Component
    {
        private Entity _entity;

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
