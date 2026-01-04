
using System.Collections.Generic;

namespace UselessFrame.Runtime.Collections
{
    public class DataProvider : IDataProvider
    {
        private Dictionary<string, object> _refData;
        private Dictionary<string, int> _intData;
        private Dictionary<string, bool> _boolData;
        private Dictionary<string, float> _floatData;
        private Dictionary<string, long> _longData;

        public T Get<T>(string key, T defaultValue = default)
        {
            if (_refData == null)
                _refData = new Dictionary<string, object>();
            if (_refData.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        public void Set(string key, object v)
        {
            if (_refData == null)
                _refData = new Dictionary<string, object>();
            _refData[key] = v;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (_boolData == null)
                _boolData = new Dictionary<string, bool>();
            if (_boolData.TryGetValue(key, out bool value))
                return value;
            return defaultValue;
        }

        public float GetFloat(string key, float defaultValue = 0)
        {
            if (_floatData == null)
                _floatData = new Dictionary<string, float>();
            if (_floatData.TryGetValue(key, out float value))
                return value;
            return defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if (_intData == null)
                _intData = new Dictionary<string, int>();
            if (_intData.TryGetValue(key, out int value))
                return value;
            return defaultValue;
        }

        public long GetLong(string key, long defaultValue = 0)
        {
            if (_longData == null)
                _longData = new Dictionary<string, long>();
            if (_longData.TryGetValue(key, out long value))
                return value;
            return defaultValue;
        }

        public void Remove(string key)
        {
            _refData?.Remove(key);
            _intData?.Remove(key);
            _boolData?.Remove(key);
            _floatData?.Remove(key);
            _longData?.Remove(key);
        }

        public void SetBool(string key, bool v)
        {
            if (_boolData == null)
                _boolData = new Dictionary<string, bool>();
            _boolData[key] = v;
        }

        public void SetFloat(string key, float v)
        {
            if (_floatData == null)
                _floatData = new Dictionary<string, float>();
            _floatData[key] = v;
        }

        public void SetInt(string key, int v)
        {
            if (_intData == null)
                _intData = new Dictionary<string, int>();
            _intData[key] = v;
        }

        public void SetLong(string key, long v)
        {
            if (_longData == null)
                _longData = new Dictionary<string, long>();
            _longData[key] = v;
        }
    }
}
