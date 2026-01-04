
using UselessFrame.Runtime.Pools;

namespace XFrame.Core
{
    /// <summary>
    /// 解析器
    /// </summary>
    public interface IParser : IPoolObject
    {
        /// <summary>
        /// 值
        /// </summary>
        object Value { get; }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">源数据</param>
        /// <returns>结果</returns>
        object Parse(string pattern);
    }

    /// <summary>
    /// 解析器
    /// </summary>
    public interface IParser<T> : IParser
    {
        /// <summary>
        /// 值
        /// </summary>
        new T Value { get; }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="pattern">源数据</param>
        /// <returns>结果</returns>
        new T Parse(string pattern);
    }
}
