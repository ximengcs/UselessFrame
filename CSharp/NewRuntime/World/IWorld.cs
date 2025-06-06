
using UselessFrame.NewRuntime.Router;
using UselessFrame.NewRuntime.Scene;

namespace UselessFrame.NewRuntime.World
{
    public interface IWorld : IRouter
    {
        WorldType Type { get; }
        ISceneManager Scene { get; }
    }
}
