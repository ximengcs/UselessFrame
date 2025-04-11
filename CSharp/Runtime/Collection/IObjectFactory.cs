
namespace UselessFrame.Runtime.Collections
{
    public interface IObjectFactory
    {
        T Require<T>();

        void Release(object obj);
    }
}
