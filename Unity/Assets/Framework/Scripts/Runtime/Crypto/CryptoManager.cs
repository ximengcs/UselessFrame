using Cysharp.Threading.Tasks;
using System;

namespace UselessFrame.NewRuntime.Cryptos
{
    public class CryptoManager : ICryptoManager, IManagerInitializer
    {
        private const string DEFAULT_KEY = "x1df2eop";
        private const string DEFAULT_IV = "3sfd2ds4";

        #region Life Fun
        private Type m_Type;

        public async UniTask Initialize(XSetting setting)
        {
            m_Type = typeof(DefaultCryptor);
        }
        #endregion

        #region Interface
        /// <inheritdoc/>
        public ICryptor Create(string keyStr, string ivStr)
        {
            ICryptor cryptor = (ICryptor)X.Type.CreateInstance(m_Type);
            cryptor.OnInit(keyStr, ivStr);
            return cryptor;
        }

        /// <inheritdoc/>
        public ICryptor Create()
        {
            ICryptor cryptor = (ICryptor)X.Type.CreateInstance(m_Type);
            cryptor.OnInit(DEFAULT_KEY, DEFAULT_IV);
            return cryptor;
        }
        #endregion
    }
}
