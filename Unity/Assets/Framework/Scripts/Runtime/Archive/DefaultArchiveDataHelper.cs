using UselessFrame.NewRuntime;
using UselessFrame.NewRuntime.Cryptos;

namespace XFrame.Modules.Archives
{
    public class DefaultArchiveDataHelper : IArchiveDataHelper
    {
        private ICryptor _cryptor;

        public DefaultArchiveDataHelper()
        {
            _cryptor = X.Crypto.Create();
        }

        public byte[] ReadBytes(byte[] data)
        {
            _cryptor.BeginDecrypty(data);
            byte[] result = _cryptor.EndDecrypty();
            _cryptor.Dispose();
            return result;
        }

        public byte[] WriteBytes(byte[] buffer)
        {
            _cryptor.BeginEncrypt();
            _cryptor.Writer.BaseStream.Write(buffer, 0, buffer.Length);
            byte[] data = _cryptor.EndEncrypt();
            _cryptor.Dispose();
            return data;
        }

        public string ReadText(byte[] data)
        {
            _cryptor.BeginDecrypty(data);
            _cryptor.EndDecrypty();
            string result = _cryptor.Reader.ReadToEnd();
            _cryptor.Dispose();
            return result;
        }

        public byte[] WriteText(string text)
        {
            _cryptor.BeginEncrypt();
            _cryptor.Writer.Write(text);
            byte[] data = _cryptor.EndEncrypt();
            _cryptor.Dispose();
            return data;
        }
    }
}
