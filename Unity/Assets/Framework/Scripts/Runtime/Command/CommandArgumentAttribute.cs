

using System;

namespace UselessFrame.NewRuntime.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CommandArgumentAttribute : Attribute
    {
        public string Name { get; }

        public string Description { get; }

        public CommandArgumentAttribute(string name, string desc = null)
        {
            Name = name;
            Description = desc;
        }
    }
}
