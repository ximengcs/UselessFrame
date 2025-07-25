using XFrame.SimpleJSON;
using XFrame.Modules.Rand;
using XFrame.Modules.Serialize;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// Json存档基类
    /// </summary>
    public abstract class JsonArchiveBase : IJsonArchive
    {
        /// <summary>
        /// 根节点对象
        /// </summary>
        protected JSONNode m_Root;

        /// <summary>
        /// 所属存档模块
        /// </summary>
        protected IArchiveModule m_Module;

        /// <summary>
        /// 存档名
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 设置int值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">值</param>
        public void SetInt(string key, int v)
        {
            m_Root[key] = v;
        }

        /// <summary>
        /// 设置long值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">值</param>
        public void SetLong(string key, long v)
        {
            m_Root[key] = v;
        }

        /// <summary>
        /// 获取int值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果未设置过此键，则会返回0</returns>
        public int GetInt(string key, int defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            return m_Root[key];
        }

        /// <summary>
        /// 获取long值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果未设置过此键，则会返回0</returns>
        public long GetLong(string key, long defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            return m_Root[key];
        }

        /// <summary>
        /// 设置float值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">值</param>
        public void SetFloat(string key, float v)
        {
            m_Root[key] = v;
        }

        /// <summary>
        /// 设置double值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">值</param>
        public void SetDouble(string key, double v)
        {
            m_Root[key] = v;
        }

        /// <summary>
        /// 获取float值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果未设置过此键，则会返回0</returns>
        public float GetFloat(string key, float defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            return m_Root[key];
        }

        /// <summary>
        /// 获取double值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果未设置过此键，则会返回0</returns>
        public float GetDouble(string key, float defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            return m_Root[key];
        }

        /// <summary>
        /// 设置bool值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">值</param>
        public void SetBool(string key, bool v)
        {
            m_Root[key] = v;
        }

        /// <summary>
        /// 获取bool值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>如果未设置过此键，则会返回false</returns>
        public bool GetBool(string key, bool defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            return m_Root[key];
        }

        /// <summary>
        /// 设置数据，推荐调用Get方法获取设置的值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <param name="v">此值将会被序列化并保存</param>
        public void Set(string key, object v)
        {
            m_Root[key] = JSONNode.Parse(m_Module.Domain.GetModule<ISerializeModule>().SerializeObjectToRaw(v));
        }

        /// <summary>
        /// 获取数据，推荐调用Set此类数据
        /// </summary>
        /// <typeparam name="T">数据类型，如果与Set设置的数据类型不匹配，可能导致出错</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>获取到的数据</returns>
        public T Get<T>(string key, T defaultValue = default)
        {
            if (!m_Root.HasKey(key))
                return defaultValue;
            string objStr = m_Root[key].ToString();
            if (string.IsNullOrEmpty(objStr))
                return defaultValue;
            return (T)m_Module.Domain.GetModule<ISerializeModule>().DeserializeToObject(objStr, typeof(T));
        }

        /// <summary>
        /// 获取(不存在时创建)一个JsonObject对象，即Json对象{}
        /// 可以使用该对象直接设置键值
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <returns></returns>
        public JSONObject GetOrNewObject(string key)
        {
            JSONObject node;
            if (m_Root.HasKey(key))
            {
                node = m_Root[key] as JSONObject;
                if (node != null && node.GetType() == typeof(JSONObject))
                    return node;
            }
            node = new JSONObject();
            m_Root[key] = node;
            return node;
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            if (m_Root.HasKey(key))
                m_Root.Remove(key);
        }

        /// <summary>
        /// 获取(不存在时创建)一个JsonArray对象，即Json数组对象[]
        /// 可以使用该对象直接设置数组数据
        /// </summary>
        /// <param name="key">键, 如果键已经存在，则会覆盖原始数据</param>
        /// <returns>获取到的JsonArray对象</returns>
        public JSONArray GetOrNewArray(string key)
        {
            JSONArray node;
            if (m_Root.HasKey(key))
            {
                node = m_Root[key] as JSONArray;
                if (node != null && node.GetType() == typeof(JSONArray))
                    return node;
            }
            node = new JSONArray();
            m_Root[key] = node;
            return node;
        }

        /// <inheritdoc/>
        public bool HasData<T>()
        {
            return HasData<T>("main_value");
        }

        /// <inheritdoc/>
        public bool HasData<T>(string name)
        {
            string key = $"{name}_{typeof(T).Name}";
            return m_Root.HasKey(key);
        }

        /// <inheritdoc/>
        public T GetData<T>()
        {
            return GetData<T>("main_value");
        }

        /// <inheritdoc/>
        public T GetData<T>(string name)
        {
            string key = $"{name}_{typeof(T).Name}";
            return Get<T>(key);
        }

        /// <inheritdoc/>
        public void SetData<T>(T value)
        {
            SetData("main_value", value);
        }

        /// <inheritdoc/>
        public void SetData<T>(string name, T value)
        {
            string key = $"{name}_{typeof(T).Name}";
            Set(key, value);
        }

        /// <inheritdoc/>
        public IJsonArchive SpwanDataProvider(string name)
        {
            return new SubJsonArchive(this, name);
        }

        /// <inheritdoc/>
        public IJsonArchive SpwanDataProvider()
        {
            return SpwanDataProvider(m_Module.Domain.GetModule<IRandModule>().RandString());
        }

        /// <inheritdoc/>
        public abstract void ClearData();
    }
}
