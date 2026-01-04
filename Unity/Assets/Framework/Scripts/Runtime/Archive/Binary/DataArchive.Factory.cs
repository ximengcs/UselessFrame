
namespace XFrame.Modules.Archives
{
    public partial class DataArchive
    {
        /// <summary>
        /// 从给定字节数据构建存档
        /// </summary>
        /// <param name="data">存档字节数据</param>
        /// <returns>构建的存档</returns>
        public static DataArchive FromBytes(byte[] data)
        {
            DataArchive archive = new DataArchive();
            archive.m_Path = string.Empty;
            archive.m_Builder = new BytesBuilder(FILE_CODE);
            archive.m_Root = archive.m_Builder.From(data);
            return archive;
        }

        /// <summary>
        /// 根据导入路径和导出路径构建存档
        /// </summary>
        /// <param name="fromPath">导入路径，存档会将给定路径中的所有文件导入进来</param>
        /// <param name="toPath">存档的导出路径</param>
        /// <returns>构建的存档</returns>
        public static DataArchive LoadPath(string fromPath, string toPath)
        {
            DataArchive archive = new DataArchive();
            archive.m_Path = toPath;
            archive.m_Builder = new BytesBuilder(FILE_CODE);
            archive.m_Root = new Node(FileType.Directory, string.Empty);
            archive.ImportDisk(fromPath);
            return archive;
        }
    }
}
