
using System;
using System.Collections.Generic;

namespace XFrame.Core
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string valueStr) where T : Enum
        {
            EnumParser<T> parser = valueStr;
            T result = parser;
            return result;
        }

        public static List<T> ToEnumList<T>(this string valueStr) where T : Enum
        {
            var parser = new ArrayParser<EnumParser<T>>();
            parser.Parse(valueStr);
            List<T> values = new List<T>();
            foreach (EnumParser<T> p in parser.Value)
                values.Add(p);
            return values;
        }
    }
}
