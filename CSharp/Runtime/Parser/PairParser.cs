using System;
using XFrame.Modules.Diagnotics;
using XFrame.Modules.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 键值项解析器
    /// </summary>
    /// <typeparam name="K">键解析器类型</typeparam>
    /// <typeparam name="V">值解析器类型</typeparam>
    public class PairParser<K, V> : IParser<Pair<K, V>>, ICanConfigLog where K : IParser where V : IParser
    {
        private const char SPLIT = '|';
        private char m_Split;
        private string m_Origin;
        private IParser m_KParser;
        private IParser m_VParser;

        /// <summary>
        /// Log等级
        /// </summary>
        public LogLevel LogLv
        {
            get => throw new NotSupportedException();
            set
            {
                ICanConfigLog configer = m_KParser as ICanConfigLog;
                if (configer != null)
                    configer.LogLv = value;
                configer = m_VParser as ICanConfigLog;
                if (configer != null)
                    configer.LogLv = value;
            }
        }

        /// <summary>
        /// 键值分割符
        /// </summary>
        public char Split
        {
            get => m_Split;
            set
            {
                if (m_Split != value)
                {
                    m_Split = value;
                    Parse(m_Origin);
                }
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public PairParser()
        {
            m_Split = SPLIT;
            InnerInitParser();
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="splitchar">键值分隔符</param>
        public PairParser(char splitchar)
        {
            m_Split = splitchar;
            InnerInitParser();
        }

        private void InnerInitParser()
        {
            m_KParser = (IParser)References.Require(typeof(K));
            m_VParser = (IParser)References.Require(typeof(V));
            Value = new Pair<K, V>((K)m_KParser, (V)m_VParser);
        }

        /// <summary>
        /// 持有键值项
        /// </summary>
        public Pair<K, V> Value { get; private set; }

        object IParser.Value => Value;

        int IPoolObject.PoolKey => default;

        /// <inheritdoc/>
        public string MarkName { get; set; }

        IPool IPoolObject.InPool { get; set; }

        /// <inheritdoc/>
        public Pair<K, V> Parse(string pattern)
        {
            m_Origin = pattern;
            if (!string.IsNullOrEmpty(pattern))
            {
                string[] values = pattern.Split(m_Split);
                if (values.Length == 1)
                {
                    m_KParser.Parse(values[0]);
                    m_VParser.Parse(null);
                }
                else if (values.Length == 2)
                {
                    m_KParser.Parse(values[0]);
                    m_VParser.Parse(values[1]);
                }
            }
            return Value;
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        /// <summary>
        /// 键值项字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// 返回哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 检查两个值是否相等
        /// </summary>
        /// <param name="obj">对比值</param>
        /// <returns>true表示相等</returns>
        public override bool Equals(object obj)
        {
            IParser parser = obj as IParser;
            return parser != null ? Value.Equals(parser.Value) : Value.Equals(obj);
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            References.Release(this);
        }

        void IPoolObject.OnCreate()
        {

        }

        void IPoolObject.OnRequest()
        {
            m_Split = SPLIT;
        }

        void IPoolObject.OnRelease()
        {
            m_KParser.OnRelease();
            m_VParser.OnRelease();
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
        public static bool operator ==(PairParser<K, V> src, object tar)
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
        /// <returns>不相等</returns>
        public static bool operator !=(PairParser<K, V> src, object tar)
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
        /// 键值项转为解析器
        /// </summary>
        /// <param name="value">键值项</param>
        public static implicit operator PairParser<K, V>(Pair<K, V> value)
        {
            PairParser<K, V> parser = References.Require<PairParser<K, V>>();
            parser.Value = value;
            return parser;
        }

        /// <summary>
        /// 返回解析器的值
        /// </summary>
        /// <param name="value">解析器</param>
        public static implicit operator Pair<K, V>(PairParser<K, V> value)
        {
            return value.Value;
        }
    }
}
