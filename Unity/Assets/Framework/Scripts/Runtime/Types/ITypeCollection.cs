
using System;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Types
{
    public interface ITypeCollection : IReadOnlyList<Type>
    {
        Type MainType { get; }

        Type Get(string typeFullName);

        Type[] ToArray();
    }
}
