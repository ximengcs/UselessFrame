
using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using UselessFrame.Runtime.Types;

namespace UselessFrame.NewRuntime.Commands
{
    public class CommandManager : ICommandManager
    {
        private Dictionary<string, CommandInfo> _commands;

        public CommandManager()
        {
            _commands = new Dictionary<string, CommandInfo>();
            ITypeCollection collection = X.Type.GetCollection(typeof(CommandClassAttribute));
            foreach (Type type in collection)
            {
                object inst = null;
                if (!type.IsAbstract || !type.IsSealed)
                    inst = Activator.CreateInstance(type);
                object cmdObject = X.Type.CreateInstance(type);

                MethodInfo[] methods = type.GetMethods();
                foreach (MethodInfo method in methods)
                {
                    CommandAttribute cmdAttr = method.GetCustomAttribute<CommandAttribute>();
                    if (cmdAttr != null)
                    {
                        CommandInfo info = new CommandInfo(inst, method, cmdAttr);
                        _commands.Add(info.Name, info);
                    }
                }
            }
        }

        public CommandExecuteResult Execute(string cmd)
        {
            string cmdName = CommandLineParser.SplitCommandLine(cmd).First();
            if (_commands.TryGetValue(cmdName, out CommandInfo info))
            {
                return info.TryExecute(cmd);
            }
            else
            {
                return new CommandExecuteResult(CommandExecuteCode.CommandNotFound);
            }
        }
    }
}
