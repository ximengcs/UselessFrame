
using System;

namespace UselessFrame.NewRuntime.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CommandOptionAttribute : Attribute
    {
        public string Name { get; }

        public string Description { get; }

        public string[] Alias { get; }

        public CommandOptionAttribute(string name, string desc = null, params string[] alias)
        {
            Name = name;
            Description = desc;
            Alias = alias;
        }
    }
}
