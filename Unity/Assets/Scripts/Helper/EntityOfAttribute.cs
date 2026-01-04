
using System;

namespace TestGame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityOfAttribute : Attribute
    {
        public Type Type { get; }

        public EntityOfAttribute(Type type)
        {
            Type = type;
        }
    }
}
