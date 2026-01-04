
using XFrame.Core;

namespace UselessFrame.NewRuntime.Cryptos
{
    /// <summary>
    /// 数据加密模块
    /// </summary>
    public interface ICryptoManager
    {
        /// <summary>
        /// 创建加密器
        /// </summary>
        /// <param name="keyStr">密钥</param>
        /// <param name="ivStr">密钥</param>
        /// <returns>加密器</returns>
        ICryptor Create(string keyStr, string ivStr);

        /// <summary>
        /// 使用默认密钥创建加密器
        /// </summary>
        /// <returns>加密器</returns>
        ICryptor Create();
    }
}
