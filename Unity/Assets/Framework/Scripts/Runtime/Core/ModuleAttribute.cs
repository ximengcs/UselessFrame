
using System;

namespace UselessFrame.Runtime
{
    public class ModuleAttribute : Attribute
    {
        public Type Type { get; }

        public ModuleAttribute(Type type)
        {
            Type = type;
        }
    }
}
