using XFrame.Core;
using XFrame.SimpleJSON;

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
        /// 设置整数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetInt(string key, int v);

        /// <summary>
        /// 设置长整型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetLong(string key, long v);

        /// <summary>
        /// 获取整数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        int GetInt(string key, int defaultValue = default);

        /// <summary>
        /// 获取长整型
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        long GetLong(string key, long defaultValue = default);

        /// <summary>
        /// 设置浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetFloat(string key, float v);

        /// <summary>
        /// 设置双精度浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetDouble(string key, double v);

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        float GetFloat(string key, float defaultValue = default);

        /// <summary>
        /// 设置双精度浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        float GetDouble(string key, float defaultValue = default);

        /// <summary>
        /// 设置布尔值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void SetBool(string key, bool v);

        /// <summary>
        /// 获取布尔值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        bool GetBool(string key, bool defaultValue = default);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="v">值</param>
        void Set(string key, object v);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        T Get<T>(string key, T defaultValue = default);

        /// <summary>
        /// 获取或创建值对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值对象</returns>
        JSONObject GetOrNewObject(string key);

        /// <summary>
        /// 获取或创建数组对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>数组对象</returns>
        JSONArray GetOrNewArray(string key);

        /// <summary>
        /// 移除一个值
        /// </summary>
        /// <param name="key">键</param>
        void Remove(string key);

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
