
namespace UselessFrame.NewRuntime.World
{
    public interface IWorld
    {
        WorldType Type { get; }
        IRouter Router { get; }
    }
}
