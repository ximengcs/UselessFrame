using System.IO;
using XFrame.SimpleJSON;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// Json存档
    /// </summary>
    [Archive("json")]
    public class JsonArchive : JsonArchiveBase, IJsonArchive, IArchive
    {
        #region Inner Fields
        private string m_Path;
        #endregion

        #region Archive Interface
        void IArchive.OnInit(IArchiveModule module, string path, string name, object param)
        {
            Name = name;
            m_Path = path;
            m_Module = module;
            if (File.Exists(m_Path))
            {
                m_Root = JSONNode.Parse(ArchiveUtility.ReadText(m_Module, m_Path));
            }
            if (m_Root == null)
                m_Root = new JSONObject();
        }

        /// <inheritdoc/>
        public void Save()
        {
            ArchiveUtility.WriteText(m_Module, m_Path, m_Root.ToString(4));
        }

        /// <inheritdoc/>
        public void Delete()
        {
            if (File.Exists(m_Path))
                File.Delete(m_Path);
        }
        #endregion

        /// <inheritdoc/>
        public override void ClearData()
        {
            m_Module.Delete(this);
        }
    }
}
