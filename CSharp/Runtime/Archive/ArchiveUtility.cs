using XFrame.Modules.Config;
using XFrame.Modules.Crypto;

namespace XFrame.Modules.Archives
{
    internal class ArchiveUtility
    {
        public static IArchiveUtilityHelper Helper { get; set; }

        public static byte[] ReadBytes(IArchiveModule module, string path)
        {
            byte[] result;
            if (XConfig.ArchiveEncrypt)
            {
                byte[] data = Helper.ReadAllBytes(path);
                ICryptor cryptor = module.Domain.GetModule<ICryptoModule>().New();
                cryptor.BeginDecrypty(data);
                result = cryptor.EndDecrypty();
                cryptor.Dispose();
            }
            else
            {
                result = Helper.ReadAllBytes(path);
            }
            return result;
        }

        public static void WriteBytes(IArchiveModule module, string path, byte[] buffer)
        {
            if (XConfig.ArchiveEncrypt)
            {
                ICryptor cryptor = module.Domain.GetModule<ICryptoModule>().New();
                cryptor.BeginEncrypt();
                cryptor.Writer.BaseStream.Write(buffer, 0, buffer.Length);
                byte[] data = cryptor.EndEncrypt();
                Helper.WriteAllBytes(path, data);
                cryptor.Dispose();
            }
            else
            {
                Helper.WriteAllBytes(path, buffer);
            }
        }

        public static string ReadText(IArchiveModule module, string path)
        {
            string result;
            if (XConfig.ArchiveEncrypt)
            {
                byte[] data = Helper.ReadAllBytes(path);
                ICryptor cryptor = module.Domain.GetModule<ICryptoModule>().New();
                cryptor.BeginDecrypty(data);
                cryptor.EndDecrypty();
                result = cryptor.Reader.ReadToEnd();
                cryptor.Dispose();
            }
            else
            {
                result = Helper.ReadAllText(path);
            }
            return result;
        }

        public static void WriteText(IArchiveModule module, string path, string text)
        {
            if (XConfig.ArchiveEncrypt)
            {
                ICryptor cryptor = module.Domain.GetModule<ICryptoModule>().New();
                cryptor.BeginEncrypt();
                cryptor.Writer.Write(text);
                byte[] data = cryptor.EndEncrypt();
                Helper.WriteAllBytes(path, data);
                cryptor.Dispose();
            }
            else
            {
                Helper.WriteAllText(path, text);
            }
        }
    }
}
