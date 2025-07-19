
namespace UselessFrame.NewRuntime.Entities
{
    public interface IUpdateSystem
    {
    }

    public interface IUpdateSystem<T> : IUpdateSystem where T : EntityComponent
    {
        void OnUpdate(T oldComp, T newComp);
    }

    public delegate void OnUpdateDelegate<T>(T comp, T comp2);
}
