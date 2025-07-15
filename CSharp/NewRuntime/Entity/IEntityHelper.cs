
using UselessFrame.NewRuntime.Worlds;

namespace UselessFrame.NewRuntime.Entities
{
    public interface IEntityHelper
    {
        void Bind(World world);

        void OnCreateEntity(Entity entity);

        void OnDestroyEntity(Entity entity);

        void OnCreateComponent(Component component);

        void OnDestroyComponent(Component component);
    }
}
