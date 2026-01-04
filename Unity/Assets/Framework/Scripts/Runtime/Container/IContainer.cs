
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime
{
    public interface IContainer
    {
        long Id { get; }

        IDataProvider Data { get; }

        IContainer Root { get; }

        IContainer Parent { get; }

        void Trigger<T>() where T : IContainerEventHandler;

        IContainer AddCom();

        T AddCom<T>() where T : IContainer;

        void RemoveCom(IContainer child);
    }
}
