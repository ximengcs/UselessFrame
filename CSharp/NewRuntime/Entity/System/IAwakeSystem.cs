
namespace UselessFrame.NewRuntime.Entities
{
    public interface IAwakeSystem
    {
        void OnAwake(Component comp);
    }

    public interface IAwakeSystem<T> where T : Component
    {
        void OnAwake(T comp);
    }
}
