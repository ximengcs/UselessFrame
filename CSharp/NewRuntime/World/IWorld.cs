
using UselessFrame.NewRuntime.Router;
using UselessFrame.NewRuntime.Scene;

namespace UselessFrame.NewRuntime.Worlds
{
    public interface IWorld : IRouter
    {
        WorldType Type { get; }
        ISceneManager Scene { get; }
        ILogManager Log { get; }
    }
}
