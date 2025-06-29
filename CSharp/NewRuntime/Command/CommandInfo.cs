using System;
using System.Reflection;
using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.IO;
using System.CommandLine.Parsing;

namespace UselessFrame.NewRuntime.Commands
{
    internal partial class CommandInfo : SynchronousCommandLineAction
    {
        private object _inst;
        private Command _command;
        private MethodInfo _method;
        private ParamInfo[] _params;
        private object[] _paramCache;
        private CommandHandle _executeHandle;

        public string Name => _command.Name;

        public CommandInfo(object inst, MethodInfo method, CommandAttribute attr)
        {
            _inst = inst;
            _method = method;
            string name = string.IsNullOrEmpty(attr.Name) ? method.Name : attr.Name;
            _command = new Command(name, attr.Description);
            _command.Add(new HelpOption());
            _command.Add(new VersionOption());
            _command.Action = this;

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                ParameterInfo p = parameters[0];
                if (p.ParameterType == typeof(CommandHandle))
                    _executeHandle = new CommandHandle();
            }

            _params = new ParamInfo[parameters.Length];
            _paramCache = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo param = parameters[i];
                if (param.ParameterType == typeof(CommandHandle))
                    continue;
                CommandArgumentAttribute argAttr = param.GetCustomAttribute<CommandArgumentAttribute>();
                if (argAttr != null)
                {
                    Argument arg = CreateArgument(argAttr, param);
                    _params[i] = new ParamInfo(param, arg);
                    _command.Add(arg);
                }
                else
                {
                    CommandOptionAttribute optAttr = param.GetCustomAttribute<CommandOptionAttribute>();
                    if (optAttr != null)
                    {
                        Option option = CreateOption(optAttr, param);
                        _params[i] = new ParamInfo(param, option);
                        _command.Add(option);
                    }
                    else
                    {
                        if (param.IsOptional)
                        {
                            Option option = CreateOption(null, param);
                            _params[i] = new ParamInfo(param, option);
                            _command.Add(option);
                        }
                        else
                        {
                            Argument arg = CreateArgument(null, param);
                            _params[i] = new ParamInfo(param, arg);
                            _command.Add(arg);
                        }
                    }
                }
            }
        }

        public CommandExecuteResult TryExecute(string cmd)
        {
            ParseResult result = _command.Parse(cmd);
            if (result.Errors.Count > 0)
            {
                TextWriter output = result.Configuration.Output;
                StringWriter writer = new StringWriter();
                foreach (ParseError error in result.Errors)
                    writer.WriteLine(error.Message);
                result.Configuration.Output = writer;
                result.Invoke();
                result.Configuration.Output = output;
                return new CommandExecuteResult(CommandExecuteCode.FormatError, writer.ToString());
            }
            else
            {
                try
                {
                    result.Invoke();
                    if (_executeHandle != null)
                        return _executeHandle.Result;
                    else
                        return new CommandExecuteResult(CommandExecuteCode.OK);
                }
                catch (Exception e)
                {
                    return new CommandExecuteResult(CommandExecuteCode.ExecuteException, e.ToString());
                }
            }
        }

        public override int Invoke(ParseResult parseResult)
        {
            int startIndex = 0;
            if (_executeHandle != null)
            {
                _paramCache[0] = _executeHandle;
                startIndex++;
            }

            for (int i = startIndex; i < _params.Length; i++)
            {
                ParamInfo entry = _params[i];
                object value = entry.GetValue(parseResult);
                _paramCache[i] = value;
            }

            if (_method.IsStatic)
                _method.Invoke(null, _paramCache);
            else
                _method.Invoke(_inst, _paramCache);
            return 0;
        }

        private Argument CreateArgument(CommandArgumentAttribute attr, ParameterInfo paramInfo)
        {
            Type type = typeof(Argument<>).MakeGenericType(paramInfo.ParameterType);
            string name = attr != null ? attr.Name : paramInfo.Name;
            string desc = attr != null ? attr.Description : $"{paramInfo.ParameterType.Name}";
            Argument arg = (Argument)Activator.CreateInstance(type, name);
            arg.Description = desc;
            return arg;
        }

        private Option CreateOption(CommandOptionAttribute attr, ParameterInfo paramInfo)
        {
            Type type = typeof(Option<>).MakeGenericType(paramInfo.ParameterType);
            string name = attr != null ? attr.Name : paramInfo.Name;
            string desc = attr != null ? attr.Description : $"{paramInfo.ParameterType.Name}";
            string[] alias = attr != null ? attr.Alias : null;
            Option opt = (Option)Activator.CreateInstance(type, name, alias);
            opt.Description = desc;
            return opt;
        }
    }
}
