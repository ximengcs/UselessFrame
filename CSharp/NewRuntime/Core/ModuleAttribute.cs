
using System;

namespace UselessFrame.Runtime
{
    public class ModuleAttribute : Attribute
    {
        public int Id { get; }

        public ModuleAttribute(int id)
        {
            Id = id;
        }
    }
}
