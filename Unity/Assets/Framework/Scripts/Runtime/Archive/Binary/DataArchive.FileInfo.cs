using System.Runtime.InteropServices;

namespace XFrame.Modules.Archives
{
    public partial class DataArchive
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct FileInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Name;
            public FileType Type;
            public int FileCount;
            public long Offset;
            public int Size;
        }
    }
}
