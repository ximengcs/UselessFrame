
using UselessFrame.NewRuntime.Entities;

namespace UselessFrame.NewRuntime.Worlds
{
    public interface IWorldHelper
    {
        void OnInit();

        void OnDispose();

        IEntityHelper CreateHelper();
    }
}
