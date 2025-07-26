using System;
using System.IO;
using UselessFrame.NewRuntime;
using System.Collections.Generic;
using UselessFrame.NewRuntime.Timers;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档模块
    /// </summary>
    public class ArchiveModule : IArchiveModule, IManagerInitializer, IManagerDisposable
    {
        #region Inner Field
        private const int SAVE_KEY = 0;
        private const int SAVE_GAP = 60;

        private string m_RootPath;
        private ITimeRecord m_Timer;
        private IFileHelper _fileHelper;
        private Dictionary<string, IArchive> m_Archives;
        private Dictionary<string, Type> m_ArchiveTypes;
        #endregion

        #region Life Fun
        public void Initialize(XSetting setting)
        {
            m_RootPath = setting.ArchivePath;
            m_Timer = X.Pool.Require<ITimeRecord>();
            m_Timer.Record(SAVE_KEY, SAVE_GAP);
            m_Archives = new Dictionary<string, IArchive>();
            m_ArchiveTypes = new Dictionary<string, Type>();

            Type type = typeof(DefaultArchiveUtilityHelper);
            _fileHelper = (IFileHelper)X.Type.CreateInstance(type);
            InnerInit();
        }

        private void InnerInit()
        {
            var system = X.Type.GetCollection(typeof(ArchiveAttribute));
            foreach (Type type in system)
                InnerAddType(type);
            InnerRefreshFiles();
        }

        private void InnerAddType(Type type)
        {
            ArchiveAttribute attri = (ArchiveAttribute)X.Type.GetAttribute(type, typeof(ArchiveAttribute));
            if (attri != null)
            {
                if (!m_ArchiveTypes.ContainsKey(attri.Suffix))
                    m_ArchiveTypes.Add(attri.Suffix, type);
            }
        }

        private void InnerRefreshFiles()
        {
            if (!string.IsNullOrEmpty(m_RootPath))
            {
                if (Directory.Exists(m_RootPath))
                    InnerInitRootPath();
                else
                    Directory.CreateDirectory(m_RootPath);
            }
        }

        /// <inheritdoc/>
        public void OnUpdate(double escapeTime)
        {
            if (m_Timer.Check(SAVE_KEY, true))
                InnerSaveAll();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            InnerSaveAll();
        }
        #endregion

        #region Interface
        /// <inheritdoc/>
        public T GetOrNew<T>(string name, object param = null) where T : IArchive
        {
            return (T)InnerGetOrNew(name, typeof(T), param);
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            InnerSaveAll();
        }

        /// <inheritdoc/>
        public void Delete(string name)
        {
            if (m_Archives.TryGetValue(name, out IArchive source))
            {
                source.Delete();
                m_Archives.Remove(name);
            }
        }

        /// <inheritdoc/>
        public void Delete(IArchive archive)
        {
            if (m_Archives.ContainsKey(archive.Name))
            {
                archive.Delete();
                m_Archives.Remove(archive.Name);
            }
        }

        /// <inheritdoc/>
        public void DeleteAll()
        {
            foreach (IArchive archive in m_Archives.Values)
            {
                archive.Delete();
            }
            m_Archives.Clear();
        }
        #endregion

        #region Inner Implement
        private string InnerGetPath(Type type, string name)
        {
            ArchiveAttribute attri = (ArchiveAttribute)X.Type.GetAttribute(type, typeof(ArchiveAttribute));
            return Path.Combine(m_RootPath, $"{name}{attri.Suffix}");
        }

        private void InnerSaveAll()
        {
            foreach (IArchive archive in m_Archives.Values)
                archive.Save();
        }

        private void InnerInitRootPath()
        {
            foreach (string file in Directory.EnumerateFiles(m_RootPath))
            {
                string suffix = Path.GetExtension(file).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (m_ArchiveTypes.TryGetValue(suffix, out Type archiveType))
                    InnerGetOrNew(fileName, archiveType, null);
            }
        }

        private IArchive InnerGetOrNew(string name, Type archiveType, object param)
        {
            if (m_Archives.TryGetValue(name, out IArchive archieve))
            {
                return archieve;
            }
            else
            {
                IArchive source = (IArchive)X.Type.CreateInstance(archiveType);
                source.OnInit(_fileHelper, InnerGetPath(archiveType, name), name, param);
                m_Archives.Add(name, source);
                return source;
            }
        }
        #endregion
    }
}
