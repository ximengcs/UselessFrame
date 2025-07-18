
namespace UselessFrame.NewRuntime.Entities
{
    public interface IUpdateSystem
    {
        void OnUpdate(Component oldComp, Component newComp);
    }

    public interface IUpdateSystem<T> where T : Component
    {
        void OnUpdate(T oldComp, T newComp);
    }
}
