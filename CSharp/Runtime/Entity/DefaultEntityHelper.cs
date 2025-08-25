
using UselessFrame.Net;

namespace UselessFrame.NewRuntime.ECS
{
    internal class DefaultEntityHelper : IEntityHelper
    {
        public INetNode NetNode => null;

        public World World => null;

        public void Bind(World world)
        {
        }

        public void OnCreateComponent(EntityComponent component)
        {
        }

        public void OnCreateEntity(Entity entity)
        {
        }

        public void OnDestroyComponent(EntityComponent component)
        {
        }

        public void OnDestroyEntity(Entity entity)
        {
        }

        public void OnUpdateComponent(EntityComponent component)
        {
        }
    }
}
