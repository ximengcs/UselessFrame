
namespace UselessFrame.Runtime
{
    public interface IModule
    {
        int Id { get; }

        IFrameCore Core { get; }
    }
}
