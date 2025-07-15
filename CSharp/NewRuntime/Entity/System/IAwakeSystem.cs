
namespace UselessFrame.NewRuntime.Entities
{
    public interface IAwakeSystem
    {
        void OnInit(Entity entity);

        void OnAwake(Component comp);

        void OnDestroy(Entity entity);
    }

    public interface IAwakeSystem<T> where T : Component
    {
        void OnAwake(T comp);
    }
}
