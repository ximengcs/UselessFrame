
namespace UselessFrame.NewRuntime.ECS
{
    public interface IWorldHelper
    {
        void OnInit();

        void OnDispose();

        IEntityHelper CreateHelper();
    }
}
