
using System;

namespace UselessFrame.NewRuntime.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }

        public string Description { get; }

        public CommandAttribute(string name = null, string desc = null)
        {
            Name = name;
            Description = desc;
        }
    }
}
