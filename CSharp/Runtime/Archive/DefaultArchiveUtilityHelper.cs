using System.IO;

namespace XFrame.Modules.Archives
{
    internal class DefaultArchiveUtilityHelper : IFileHelper
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

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            return File.Delete(path);
        }
    }
}
