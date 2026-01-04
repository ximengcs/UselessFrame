using XFrame.Core;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档模块接口
    /// </summary>
    public interface IArchiveModule : ISaveable
    {
        /// <summary>
        /// 获取或创建一个存档实例
        /// </summary>
        /// <typeparam name="T">存档类型</typeparam>
        /// <param name="name">存档名</param>
        /// <param name="param">参数</param>
        /// <returns>存档实例</returns>
        T GetOrNew<T>(string name, object param = null) where T : IArchive;

        /// <summary>
        /// 删除一份存档
        /// </summary>
        /// <param name="name">存档名</param>
        void Delete(string name);

        /// <summary>
        /// 删除一份存档
        /// </summary>
        /// <param name="archive">存档实例</param>
        void Delete(IArchive archive);

        /// <summary>
        /// 删除所有存档 
        /// </summary>
        void DeleteAll();
    }
}
