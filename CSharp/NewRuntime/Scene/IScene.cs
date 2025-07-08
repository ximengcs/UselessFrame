using UselessFrame.NewRuntime.Entity;
using UselessFrame.NewRuntime.Router;

namespace UselessFrame.NewRuntime.Scene
{
    public interface IScene
    {
        IEntityManager Entity { get; }
    }
}
