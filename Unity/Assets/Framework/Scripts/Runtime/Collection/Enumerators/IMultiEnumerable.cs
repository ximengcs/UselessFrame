
using System.Collections;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Collections
{
    public interface IMultiEnumerable : IEnumerable
    {
        IEnumerator GetEnumerator(EnumeratorType type);
    }

    public interface IMultiEnumerable<T> : IEnumerable<T>, IMultiEnumerable
    {
        new IEnumerator<T> GetEnumerator(EnumeratorType type);
    }
}
