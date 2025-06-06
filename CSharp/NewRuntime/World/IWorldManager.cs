using UselessFrame.NewRuntime.Router;

namespace UselessFrame.NewRuntime.World
{
    public interface IWorldManager : IRouter
    {
        IWorld GetWorld(int worldId);
    }
}
