
namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档模块辅助器
    /// </summary>
    public interface IArchiveUtilityHelper
    {
        /// <summary>
        /// 读取二进制数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>二进制数据</returns>
        byte[] ReadAllBytes(string path);

        /// <summary>
        /// 写入二进制数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="data">二进制数据</param>
        void WriteAllBytes(string path, byte[] data);

        /// <summary>
        /// 读取文本数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>文本</returns>
        string ReadAllText(string path);

        /// <summary>
        /// 写入文本数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="data">文本</param>
        void WriteAllText(string path, string data);
    }
}
