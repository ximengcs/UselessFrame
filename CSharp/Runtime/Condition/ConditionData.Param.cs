
using System;
using XFrame.Core;
using UselessFrame.NewRuntime;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace XFrame.Modules.Conditions
{
    public partial struct ConditionData
    {
        public class Param
        {
            private string _raw;

            private int _intValue;
            private bool _intDirty;

            private float _floatValue;
            private bool _floatDirty;

            private bool _boolValue;
            private bool _boolDirty;

            private Dictionary<Type, bool> _dirties;
            private Dictionary<Type, object> _values;

            public int IntValue
            {
                get
                {
                    if (!_intDirty)
                        return _intValue;

                    var pool = X.Pool.GetOrNew<IntParser>();
                    IntParser parser = pool.Require();
                    _intValue = parser.Parse(_raw);
                    _intDirty = false;
                    pool.Release(parser);
                    return _intValue;
                }
            }

            public float FloatValue
            {
                get
                {
                    if (!_floatDirty)
                        return _floatValue;

                    var pool = X.Pool.GetOrNew<FloatParser>();
                    FloatParser parser = pool.Require();
                    _floatValue = parser.Parse(_raw);
                    _floatDirty = false;
                    pool.Release(parser);
                    return _intValue;
                }
            }

            public bool BoolValue
            {
                get
                {
                    if (!_boolDirty)
                        return _boolValue;

                    var pool = X.Pool.GetOrNew<BoolParser>();
                    BoolParser parser = pool.Require();
                    _boolValue = parser.Parse(_raw);
                    _boolDirty = false;
                    pool.Release(parser);
                    return _boolValue;
                }
            }

            public Param(string raw)
            {
                _raw = raw;
                _intDirty = true;
            }

            public T As<T, ParserT>() where T : class where ParserT : IParser<T>
            {
                if (_values == null)
                {
                    _values = new Dictionary<Type, object>();
                    _dirties = new Dictionary<Type, bool>();
                }

                if (_dirties.TryGetValue(typeof(T), out bool dirty) && !dirty)
                {
                    return (T)_values[typeof(T)];
                }    

                IPool<ParserT> pool = X.Pool.GetOrNew<ParserT>();
                IParser<T> parser = pool.Require();
                T value = parser.Parse(_raw);
                _values[typeof(T)] = value;
                _dirties[typeof(T)] = false;
                return value;
            }
        }
    }
}
