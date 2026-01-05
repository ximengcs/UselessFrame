
using System;

namespace UselessFrame.Runtime.Types
{
    internal class DefaultTypeFilter : ITypeFilter
    {
        public bool CheckAssembly(string assemblyName)
        {
            if (assemblyName.StartsWith("Microsoft") ||
                assemblyName.StartsWith("System") ||
                assemblyName.StartsWith("Unity"))
                return false;
            return true;
        }

        public bool CheckType(Type type)
        {
            return true;
        }
    }
}
