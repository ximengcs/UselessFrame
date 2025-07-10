using UselessFrame.NewRuntime.Router;

namespace UselessFrame.NewRuntime.Worlds
{
    public interface IWorldManager : IRouter
    {
        IWorld GetWorld(int worldId);
    }
}
