
using System;
using System.CommandLine;
using System.Reflection;
using UselessFrame.NewRuntime.Entity;

namespace UselessFrame.NewRuntime.Commands
{
    internal partial class CommandInfo
    {
        private class ParamInfo
        {
            private object[] _param;
            public readonly ParameterInfo Pamameter;
            public readonly Symbol Symbol;
            public readonly MethodInfo GetValueMethod;
            public readonly bool IsArg;

            public ParamInfo(ParameterInfo pamameter, Symbol symbol)
            {
                Pamameter = pamameter;
                Symbol = symbol;
                MethodInfo m = typeof(ParseResult).GetMethod("GetValue", new Type[] { typeof(string) });
                GetValueMethod = m.MakeGenericMethod(pamameter.ParameterType);
                IsArg = symbol is Argument;
                _param = new object[] { symbol.Name };
            }

            public object GetValue(object inst)
            {
                return GetValueMethod.Invoke(inst, _param);
            }
        }
    }
}
