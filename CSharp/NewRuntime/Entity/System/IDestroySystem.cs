
namespace UselessFrame.NewRuntime.ECS
{
    public interface IDestroySystem
    {
    }

    public interface IDestroySystem<T> : IDestroySystem where T : EntityComponent
    {
        void OnDestroy(T comp);
    }

    public delegate void OnDestroyDelegate<T>(T comp);
}
