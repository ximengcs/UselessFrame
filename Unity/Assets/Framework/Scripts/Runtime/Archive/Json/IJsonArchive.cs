
using Newtonsoft.Json.Linq;
using UselessFrame.Runtime.Collections;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// Json存档
    /// </summary>
    public interface IJsonArchive : IDataProvider
    {
        /// <summary>
        /// 存档名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 设置双精度浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetDouble(string key, double v);

        /// <summary>
        /// 设置双精度浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        double GetDouble(string key, double defaultValue = default);

        /// <summary>
        /// 获取或创建值对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值对象</returns>
        JObject GetOrNewObject(string key);

        /// <summary>
        /// 获取或创建数组对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>数组对象</returns>
        JArray GetOrNewArray(string key);

        /// <summary>
        /// 创建一个Json存档并作为子节点
        /// </summary>
        /// <param name="name">存档名</param>
        /// <returns>存档</returns>
        IJsonArchive SpwanDataProvider(string name);

        /// <summary>
        /// 创建一个Json存档并作为子节点
        /// </summary>
        /// <returns>存档</returns>
        IJsonArchive SpwanDataProvider();
    }
}
