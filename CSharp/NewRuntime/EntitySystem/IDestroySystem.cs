
namespace UselessFrame.NewRuntime.Entities
{
    public interface IDestroySystem
    {
        void Destroy(Component comp);
    }

    public interface IDestroySystem<T> where T : Component
    {
        void Destroy(T comp);
    }
}
