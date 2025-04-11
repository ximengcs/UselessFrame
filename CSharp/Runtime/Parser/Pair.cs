using System;
using System.Reflection;

namespace XFrame.Core
{
    /// <summary>
    /// 键值
    /// </summary>
    public class Pair
    {
        /// <summary>
        /// 创建键值项
        /// </summary>
        /// <typeparam name="K">键类型</typeparam>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>键值项</returns>
        public static Pair<K, V> Create<K, V>(K key, V value)
        {
            return new Pair<K, V>(key, value);
        }
    }

    /// <summary>
    /// 键值结构
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    public partial struct Pair<K, V>
    {
        /// <summary>
        /// 持有键
        /// </summary>
        public K Key { get; set; }

        /// <summary>
        /// 持有值
        /// </summary>
        public V Value { get; set; }

        /// <summary>
        /// 构造键值项
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public Pair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// 检查两个键值项是否相等
        /// </summary>
        /// <param name="src">对比项</param>
        /// <param name="tar">对比项</param>
        /// <returns>true表示相等</returns>
        public static bool operator ==(Pair<K, V> src, Pair<K, V> tar)
        {
            return src.Equals(tar);
        }

        /// <summary>
        /// 检查两个值是否不相等
        /// </summary>
        /// <param name="src">对比项</param>
        /// <param name="tar">对比项</param>
        /// <returns>true表示不相等</returns>
        public static bool operator !=(Pair<K, V> src, Pair<K, V> tar)
        {
            return !src.Equals(tar);
        }

        /// <summary>
        /// 检查两个键值项是否相等
        /// </summary>
        /// <param name="obj">对比项</param>
        /// <returns>true表示想等</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Pair<K, V> other)
            {
                return Key.Equals(other.Key) && Value.Equals(other.Value);
            }
            else
            {
                Type type = obj.GetType();
                if (s_GenericType == type.GetGenericTypeDefinition())
                {
                    PropertyInfo key = type.GetProperty(nameof(Key));
                    PropertyInfo value = type.GetProperty(nameof(Value));
                    return Key.Equals(key.GetValue(obj)) && Value.Equals(value.GetValue(obj));
                }
                else
                {
                    return base.Equals(obj);
                }
            }
        }

        /// <summary>
        /// 返回哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 返回键值项字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return $"[{Key},{Value}]";
        }
    }
}
