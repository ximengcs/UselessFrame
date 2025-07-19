
namespace UselessFrame.NewRuntime.Entities
{
    public interface IAwakeSystem
    {
    }

    public interface IAwakeSystem<T> : IAwakeSystem where T : EntityComponent
    {
        void OnAwake(T comp);
    }

    public delegate void OnAwakeDelegate<T>(T comp);
}
