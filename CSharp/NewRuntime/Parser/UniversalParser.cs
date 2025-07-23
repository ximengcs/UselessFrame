using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 通用解析器
    /// </summary>
    public class UniversalParser : IParser<string>
    {
        private string m_Value;
        private int m_IntValue;
        private float m_FloatValue;
        private bool m_BoolValue;
        private IPool _pool;
        private Dictionary<Type, IParser> m_Parsers;

        /// <summary>
        /// 持有原始值
        /// </summary>
        public string Value => m_Value;

        /// <summary>
        /// 持有整形值
        /// </summary>
        public int IntValue => m_IntValue;

        /// <summary>
        /// 持有浮点值 
        /// </summary>
        public float FloatValue => m_FloatValue;

        /// <summary>
        /// 持有布尔值
        /// </summary>
        public bool BoolValue => m_BoolValue;

        object IParser.Value => m_Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string Name { get; set; }

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <summary>
        /// 获取或添加解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        /// <returns>解析器</returns>
        public T GetOrAddParser<T>() where T : IParser
        {
            T parser = GetParser<T>();
            if (parser == null)
                parser = AddParser<T>();
            return parser;
        }

        /// <summary>
        /// 获取解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        /// <returns>解析器</returns>
        public T GetParser<T>() where T : IParser
        {
            if (m_Parsers.TryGetValue(typeof(T), out IParser parser))
            {
                parser.Parse(m_Value);
                return (T)parser;
            }
            return default;
        }

        /// <summary>
        /// 移除解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        public void RemoveParser<T>() where T : IParser
        {
            Type type = typeof(T);
            if (m_Parsers.TryGetValue(type, out IParser parser))
            {
                m_Parsers.Remove(type);
                IPool pool = _pool.System.GetOrNew(type);
                pool.Release(parser);
            }
        }

        /// <summary>
        /// 添加解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        /// <param name="parser">解析器</param>
        /// <returns>解析器</returns>
        public T AddParser<T>(T parser) where T : IParser
        {
            if (parser != null)
            {
                Type type = typeof(T);
                parser.Parse(m_Value);
                if (m_Parsers.TryGetValue(type, out IParser oldParser))
                {
                    //Log.Warning(Log.XFrame, $"Universal parser already has parser {type.Name}. will release old.");
                    IPool pool = _pool.System.GetOrNew(type);
                    pool.Release(oldParser);
                    m_Parsers[type] = parser;
                }
                else
                {
                    m_Parsers.Add(type, parser);
                }
            }
            return parser;
        }

        /// <summary>
        /// 添加解析器
        /// </summary>
        /// <param name="parserType">解析器类型</param>
        /// <returns>解析器</returns>
        public IParser AddParser(Type parserType)
        {
            if (!m_Parsers.TryGetValue(parserType, out IParser parser))
            {
                IPool pool = _pool.System.GetOrNew(parserType);
                parser = (IParser)pool.Require();
                parser.Parse(m_Value);
                m_Parsers.Add(parserType, parser);
            }
            return parser;
        }

        /// <summary>
        /// 添加解析器
        /// </summary>
        /// <typeparam name="T">解析器类型</typeparam>
        /// <returns>解析器</returns>
        public T AddParser<T>() where T : IParser
        {
            return (T)AddParser(typeof(T));
        }

        private UniversalParser()
        {
            m_Parsers = new Dictionary<Type, IParser>();
        }

        private void InnerInitIntValue(int value)
        {
            m_IntValue = value;
            m_Value = value.ToString();
            m_FloatValue = value;
            m_BoolValue = value == 0 ? false : true;
        }

        private void InnerInitFloatValue(float value)
        {
            m_IntValue = (int)value;
            m_Value = value.ToString();
            m_FloatValue = value;
            m_BoolValue = value == 0 ? false : true;
        }

        private void InnerInitBoolValue(bool value)
        {
            m_IntValue = value ? 1 : 0;
            m_Value = value.ToString();
            m_FloatValue = m_IntValue;
            m_BoolValue = value;
        }

        private void InnerInitStringValue(string value)
        {
            Parse(value);
        }

        private IPool<IntParser> _intPool;
        private IPool<FloatParser> _floatPool;
        private IPool<BoolParser> _boolPool;

        /// <inheritdoc/>
        public string Parse(string pattern)
        {
            m_Value = pattern;

            if (_intPool == null)
            {
                IPoolManager poolSys = _pool.System;
                _intPool = poolSys.GetOrNew<IntParser>();
                _floatPool = poolSys.GetOrNew<FloatParser>();
                _boolPool = poolSys.GetOrNew<BoolParser>();
            }

            IntParser p1 = _intPool.Require();
            FloatParser p2 = _floatPool.Require();
            BoolParser p3 = _boolPool.Require();

            m_IntValue = p1.Parse(m_Value);
            m_FloatValue = p2.Parse(m_Value);
            m_BoolValue = p3.Parse(m_Value);

            _intPool.Release(p1);
            _floatPool.Release(p2);
            _boolPool.Release(p3);

            return m_Value;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 返回原始串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return m_Value;
        }

        /// <summary>
        /// 返回原始串哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            if (parser != null)
            {
                if (m_Value != null)
                    return m_Value.Equals(parser.Value);
                else
                    return parser.Value == null;
            }
            else
            {
                if (m_Value != null)
                    return m_Value.Equals(obj);
                else
                    return obj == null;
            }
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }

        void IPoolObject.OnCreate()
        {

        }

        void IPoolObject.OnRequest()
        {

        }

        void IPoolObject.OnRelease()
        {
            foreach (var item in m_Parsers)
            {
                IPool pool = _pool.System.GetOrNew(item.Key);
                pool.Release(item.Value);
            }
            m_Parsers.Clear();
            m_Value = default;
            m_IntValue = default;
            m_FloatValue = default;
            m_BoolValue = default;
        }

        void IPoolObject.OnDelete()
        {

        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(UniversalParser src, object tar)
        {
            if (ReferenceEquals(src, null))
            {
                return ReferenceEquals(tar, null);
            }
            else
            {
                return src.Equals(tar);
            }
        }

        /// <summary>
        /// 检查两个值是否不相等
        /// </summary>
        /// <param name="src">解析器</param>
        /// <param name="tar">对比值</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(UniversalParser src, object tar)
        {
            if (ReferenceEquals(src, null))
            {
                return !ReferenceEquals(tar, null);
            }
            else
            {
                return !src.Equals(tar);
            }
        }

        /// <summary>
        /// 返回解析器字符串值
        /// </summary>
        /// <param name="parser">解析器</param>
        public static implicit operator string(UniversalParser parser)
        {
            return parser != null ? parser.m_Value : default;
        }

        /// <summary>
        /// 将字符串转换为解析器
        /// </summary>
        /// <param name="value">字符串值</param>
        public static implicit operator UniversalParser(string value)
        {
            UniversalParser parser = new UniversalParser();
            parser.InnerInitStringValue(value);
            return parser;
        }

        /// <summary>
        /// 返回解析器整形值
        /// </summary>
        /// <param name="parser">整型值</param>
        public static implicit operator int(UniversalParser parser)
        {
            return parser != null ? parser.m_IntValue : default;
        }

        /// <summary>
        /// 将整型值转换为解析器
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator UniversalParser(int value)
        {
            UniversalParser parser = new UniversalParser();
            parser.InnerInitIntValue(value);
            return parser;
        }

        /// <summary>
        /// 返回解析器的浮点值
        /// </summary>
        /// <param name="parser">浮点值</param>
        public static implicit operator float(UniversalParser parser)
        {
            return parser != null ? parser.m_FloatValue : default;
        }

        /// <summary>
        /// 将浮点值转换为解析器
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator UniversalParser(float value)
        {
            UniversalParser parser = new UniversalParser();
            parser.InnerInitFloatValue(value);
            return parser;
        }

        /// <summary>
        /// 返回解析器的布尔值
        /// </summary>
        /// <param name="parser">布尔值</param>
        public static implicit operator bool(UniversalParser parser)
        {
            return parser != null ? parser.m_BoolValue : default;
        }

        /// <summary>
        /// 将布尔值转换为解析器
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator UniversalParser(bool value)
        {
            UniversalParser parser = new UniversalParser();
            parser.InnerInitBoolValue(value);
            return parser;
        }
    }
}
