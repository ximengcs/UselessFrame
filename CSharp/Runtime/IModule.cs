using Cysharp.Threading.Tasks;

namespace UselessFrame.Runtime
{
    public interface IModule
    {
        int Id { get; }

        IModuleDriver Driver { get; }
    }
}
