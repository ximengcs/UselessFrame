﻿using System;
using System.Collections.Generic;
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 键值解析器
    /// </summary>
    /// <typeparam name="K">键解析器</typeparam>
    /// <typeparam name="V">值解析器</typeparam>
    public class MapParser<K, V> : IParser<Dictionary<K, V>> where K : IParser where V : IParser
    {
        protected IPool _pool;
        protected IPool<K> _keyPool;
        protected IPool<V> _valuePool;

        /// <summary>
        /// 默认列表项分隔符
        /// </summary>
        public static char SPLIT = ',';

        /// <summary>
        /// 默认键值分割符
        /// </summary>
        public static char SPLIT2 = '|';
        private Dictionary<K, V> m_Value;

        /// <summary>
        /// 键值字典
        /// </summary>
        public Dictionary<K, V> Value => m_Value;

        object IParser.Value => m_Value;

        IPool IPoolObject.InPool
        {
            get => _pool;
            set => _pool = value;
        }

        /// <summary>
        /// 原始字符串
        /// </summary>
        protected string m_Origin;
        private char m_Split;
        private char m_Split2;

        /// <summary>
        /// 列表项分隔符
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
        /// 键值分割符
        /// </summary>
        public char Split2
        {
            get => m_Split2;
            set
            {
                if (m_Split2 != value)
                {
                    m_Split2 = value;
                    Parse(m_Origin);
                }
            }
        }

        /// <summary>
        /// 根据键检索值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V this[K key] => m_Value[key];

        /// <summary>
        /// 键值对数量
        /// </summary>
        public int Count => m_Value.Count;

        Dictionary<K, V> IParser<Dictionary<K, V>>.Value => m_Value;

        int IPoolObject.PoolKey => default;

        /// <summary>
        /// 构造器
        /// </summary>
        protected MapParser()
        {
            m_Split = SPLIT;
            m_Split2 = SPLIT2;
            m_Value = new Dictionary<K, V>();
        }

        /// <summary>
        /// 根据键获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public V Get(K key)
        {
            if (m_Value.TryGetValue(key, out V value))
                return value;
            return default;
        }

        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>true表示存在</returns>
        public bool Has(K key)
        {
            return m_Value.ContainsKey(key);
        }

        /// <summary>
        /// 尝试解析值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否解析成功</returns>
        public bool TryGet(K key, out V value)
        {
            return m_Value.TryGetValue(key, out value);
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">文本</param>
        /// <returns>解析到的键值对列表</returns>
        public Dictionary<K, V> Parse(string pattern)
        {
            m_Origin = pattern;

            if (_keyPool == null)
            {
                Type kType = typeof(K);
                Type vType = typeof(V);
                _keyPool = (IPool<K>)_pool.System.GetOrNew(kType);
                _valuePool = (IPool<V>)_pool.System.GetOrNew(vType);
            }
            

            if (!string.IsNullOrEmpty(pattern))
            {
                pattern = pattern.Trim('{', '}');
                string[] pMap = pattern.Split(m_Split);
                for (int i = 0; i < pMap.Length; i++)
                {
                    string pItemStr = pMap[i];
                    K kParser;
                    V vParser;
                    if (!string.IsNullOrEmpty(pItemStr))
                    {
                        string[] pItem = pItemStr.Split(m_Split2);
                         InnerParseItem(out kParser, out vParser, pItem);
                    }
                    else
                    {
                        kParser = _keyPool.Require();
                        vParser = _valuePool.Require();
                    }
                    if (m_Value.ContainsKey(kParser))
                        _keyPool.Release(kParser);
                    m_Value[kParser] = vParser;
                }
            }

            return m_Value;
        }

        /// <summary>
        /// 释放到池中
        /// </summary>
        public void Release()
        {
            _pool.Release(this);
        }

        /// <summary>
        /// 解析单项
        /// </summary>
        /// <param name="kParser">键解析器</param>
        /// <param name="vParser">值解析器</param>
        /// <param name="pItem">分隔到的字符串值</param>
        protected virtual void InnerParseItem(out K kParser, out V vParser, string[] pItem)
        {
            kParser = _keyPool.Require();
            vParser = _valuePool.Require();
            kParser.Parse(pItem[0]);
            if (pItem.Length > 1)
                vParser.Parse(pItem[1]);
        }

        object IParser.Parse(string pattern)
        {
            return Parse(pattern);
        }

        void IPoolObject.OnRelease()
        {
            foreach (var item in m_Value)
            {
                _keyPool.Release(item.Key);
                _valuePool.Release(item.Value);
            }
            m_Value.Clear();
            OnRelease();
        }

        protected virtual void OnRelease()
        {

        }

        /// <summary>
        /// 返回原始字符串
        /// </summary>
        /// <returns>原始字符串</returns>
        public override string ToString()
        {
            return m_Origin;
        }

        Dictionary<K, V> IParser<Dictionary<K, V>>.Parse(string pattern)
        {
            throw new NotImplementedException();
        }

        void IPoolObject.OnCreate()
        {
            OnCreate();
        }

        protected virtual void OnCreate()
        {
        }

        void IPoolObject.OnRequest()
        {

        }

        void IPoolObject.OnDelete()
        {

        }
    }
}
