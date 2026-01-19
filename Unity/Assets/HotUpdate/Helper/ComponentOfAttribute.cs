
using System;

namespace TestGame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ComponentOfAttribute : Attribute
    {
        public Type Type { get; }

        public ComponentOfAttribute(Type type)
        {
            Type = type;
        }
    }
}
