using System;

namespace UselessFrame.Runtime.Types
{
    public interface ITypeFilter
    {
        string[] AssemblyList { get; }

        bool CheckType(Type type);
    }
}
