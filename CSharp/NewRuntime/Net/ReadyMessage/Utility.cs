
using System.Text;

namespace Core.Network
{
    public static class Utility
    {
        public static int SizeOf(this string content)
        {
            return Encoding.UTF8.GetByteCount(content);
        }
    }
}
