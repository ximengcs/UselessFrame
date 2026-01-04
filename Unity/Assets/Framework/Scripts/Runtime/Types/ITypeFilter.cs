using System;

namespace UselessFrame.Runtime.Types
{
    public interface ITypeFilter
    {
        bool CheckAssembly(string assemblyName);

        bool CheckType(Type type);
    }
}
