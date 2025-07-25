using System.IO;

namespace XFrame.Modules.Archives
{
    internal class DefaultArchiveUtilityHelper : IArchiveUtilityHelper
    {
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void WriteAllBytes(string path, byte[] buffer)
        {
            File.WriteAllBytes(path, buffer);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }
    }
}
