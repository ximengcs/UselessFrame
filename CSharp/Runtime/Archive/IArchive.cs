
namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档
    /// </summary>
    public interface IArchive
    {
        /// <summary>
        /// 存档名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 初始化生命周期
        /// </summary>
        /// <param name="module">存档所属模块</param>
        /// <param name="path">存储路径</param>
        /// <param name="name">存储名</param>
        /// <param name="data">用户参数</param>
        protected internal void OnInit(IArchiveModule module, string path, string name, object data);

        /// <summary>
        /// 保存存档
        /// </summary>
        void Save();

        /// <summary>
        /// 删除存档
        /// </summary>
        void Delete();
    }
}
