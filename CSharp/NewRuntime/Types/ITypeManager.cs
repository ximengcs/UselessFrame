
using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime
{
    public interface ITypeManager
    {
        Type this[string fullName] { get; }
        IReadOnlyList<Type> Types { get; }
        bool TryGetType(string fullName, out Type type);
        object CreateInstance(Type type, params object[] args);
        ITypeCollection GetCollection(Type type);
        Attribute GetAttribute(Type classType, Type pType);
        IReadOnlyList<Attribute> GetAttributes(Type classType);
    }
}
