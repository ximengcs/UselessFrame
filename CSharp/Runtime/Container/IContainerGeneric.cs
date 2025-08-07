
using UselessFrame.Runtime.Collections;

namespace UselessFrame.NewRuntime
{
    public interface IContainer<OwnerT>
    {
        OwnerT Owner { get; }

        long Id { get; }

        IDataProvider Data { get; }

        IContainer<OwnerT> Root { get; }

        IContainer<OwnerT> Parent { get; }

        void Trigger<T>() where T : IContainerEventHandler;

        IContainer<OwnerT> AddCom();

        T AddCom<T>() where T : IContainer<OwnerT>;

        void RemoveCom(IContainer<OwnerT> child);
    }
}
