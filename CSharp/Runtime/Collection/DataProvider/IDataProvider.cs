
namespace UselessFrame.Runtime.Collections
{
    public interface IDataProvider
    {
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
        /// 获取浮点值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值</returns>
        float GetFloat(string key, float defaultValue = default);

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
        /// 移除一个值
        /// </summary>
        /// <param name="key">键</param>
        void Remove(string key);

    }
}
