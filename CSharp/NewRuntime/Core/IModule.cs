
namespace UselessFrame.Runtime
{
    public interface IModule
    {
        int Id { get; }

        IModuleCore Core { get; }
    }
}
