using System.Collections.Generic;

namespace XFrame.Core
{
    /// <summary>
    /// 标识符扩展
    /// </summary>
    public static class NameExtension
    {
        /// <summary>
        /// 检查字典是否包含标识符
        /// </summary>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="map">字典</param>
        /// <param name="value">对比值</param>
        /// <returns>true表示存在</returns>
        public static bool ContainsName<V>(this Dictionary<Name, V> map, Name value)
        {
            bool result = map.ContainsKey(value);
            value.Release();
            return result;
        }

        /// <summary>
        /// 根据标识符获取字典中的某个值
        /// </summary>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="map">字典</param>
        /// <param name="value">标识符</param>
        /// <param name="releaseKey">自动释放键解析器</param>
        /// <returns>获取到的值</returns>
        public static V Get<V>(this Dictionary<Name, V> map, Name value, bool releaseKey = true)
        {
            V result = map[value];
            if (releaseKey)
                value.Release();
            return result;
        }

        /// <summary>
        /// 尝试根据标识符获取字典中的某个值
        /// </summary>
        /// <typeparam name="V">值类型</typeparam>
        /// <param name="map">字典</param>
        /// <param name="value">标识符</param>
        /// <param name="result">获取到的值</param>
        /// <param name="releaseKey">是否自动释放键解析器到池中</param>
        /// <returns>true表示获取成功</returns>
        public static bool TryGet<V>(this Dictionary<Name, V> map, Name value, out V result, bool releaseKey = true)
        {
            bool success = map.TryGetValue(value, out result);
            if (releaseKey)
                value.Release();
            return success;
        }
    }
}
