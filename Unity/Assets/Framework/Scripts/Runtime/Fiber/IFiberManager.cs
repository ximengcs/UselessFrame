
namespace UselessFrame.NewRuntime.Fiber
{
    public interface IFiberManager
    {
        IFiber MainFiber { get; }

        IFiber Create();
    }
}
