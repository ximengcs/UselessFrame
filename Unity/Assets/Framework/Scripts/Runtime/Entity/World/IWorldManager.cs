
namespace UselessFrame.NewRuntime.ECS
{
    public interface IWorldManager
    {
        World Create(WorldSetting setting);

        void Destroy(World world);
    }
}
