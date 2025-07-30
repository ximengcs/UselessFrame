
namespace UselessFrame.Runtime
{
    public interface IModuleCore : IModuleDriver
    {
        int Id { get; }

        void Trigger<T>(object data);

        void AddHandler<T>() where T : IModuleHandler;
    }
}
