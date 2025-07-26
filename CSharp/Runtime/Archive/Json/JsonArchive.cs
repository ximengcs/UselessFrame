using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UselessFrame.NewRuntime;

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
        void IArchive.OnInit(IFileHelper helper, string path, string name, object param)
        {
            Name = name;
            m_Path = path;
            _helper = helper;
            if (_helper.Exists(m_Path))
            {
                m_Root = JObject.Parse(_helper.ReadAllText(m_Path));
            }
            if (m_Root == null)
                m_Root = new JObject();
        }

        /// <inheritdoc/>
        public void Save()
        {
            _helper.WriteAllText(m_Path, m_Root.ToString(Formatting.Indented));
        }

        /// <inheritdoc/>
        public void Delete()
        {
            _helper.Delete(m_Path);
        }
        #endregion

        /// <inheritdoc/>
        public override void ClearData()
        {
            X.Archive.Delete(this);
        }
    }
}
