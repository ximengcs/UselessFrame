
using System;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Types
{
    public interface ITypeSystem
    {
        object CreateInstance(Type type, params object[] args);

        ITypeCollection GetOrNewWithAttr(Type pType);

        Attribute GetAttribute(Type classType, Type pType);

        IReadOnlyList<Attribute> GetAttributes(Type classType);

        ITypeCollection GetOrNew(Type baseType);
    }
}
