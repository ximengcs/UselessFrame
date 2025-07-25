using System;
using System.IO;
using XFrame.Core;
using XFrame.Modules.Reflection;
using XFrame.Modules.Times;
using XFrame.Modules.Config;
using XFrame.Modules.Crypto;
using System.Collections.Generic;
using XFrame.Collections;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档模块
    /// </summary>
    [CoreModule]
    [RequireModule(typeof(CryptoModule))]
    [XType(typeof(IArchiveModule))]
    public class ArchiveModule : ModuleBase, IArchiveModule
    {
        #region Inner Field
        private const int SAVE_KEY = 0;
        private const int SAVE_GAP = 60;

        private string m_RootPath;
        private CDTimer m_Timer;
        private Dictionary<string, IArchive> m_Archives;
        private Dictionary<string, Type> m_ArchiveTypes;
        #endregion

        #region Life Fun
        /// <inheritdoc/>
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            m_Timer = CDTimer.Create();
            m_Timer.MarkName = nameof(ArchiveModule);
            m_Timer.Record(SAVE_KEY, SAVE_GAP);
            m_Archives = new Dictionary<string, IArchive>();
            m_ArchiveTypes = new Dictionary<string, Type>();

            Type type;
            if (!string.IsNullOrEmpty(XConfig.ArchiveUtilityHelper))
            {
                type = Domain.TypeModule.GetType(XConfig.ArchiveUtilityHelper);
            }
            else
            {
                type = typeof(DefaultArchiveUtilityHelper);
            }
            IArchiveUtilityHelper helper = (IArchiveUtilityHelper)Domain.TypeModule.CreateInstance(type);
            ArchiveUtility.Helper = helper;
            InnerInit();
        }

        private void InnerInit()
        {
            TypeSystem system = Domain.TypeModule.GetOrNewWithAttr<ArchiveAttribute>();
            foreach (Type type in system)
                InnerAddType(type);
            InnerRefreshFiles();
        }

        private void InnerAddType(Type type)
        {
            ArchiveAttribute attri = Domain.TypeModule.GetAttribute<ArchiveAttribute>(type);
            if (attri != null)
            {
                if (!m_ArchiveTypes.ContainsKey(attri.Suffix))
                    m_ArchiveTypes.Add(attri.Suffix, type);
            }
        }

        private void InnerRefreshFiles()
        {
            m_RootPath = XConfig.ArchivePath;
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
        protected override void OnDestroy()
        {
            base.OnDestroy();
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
            ArchiveAttribute attri = Domain.TypeModule.GetAttribute<ArchiveAttribute>(type);
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
                IArchive source = (IArchive)Domain.TypeModule.CreateInstance(archiveType);
                source.OnInit(this, InnerGetPath(archiveType, name), name, param);
                m_Archives.Add(name, source);
                return source;
            }
        }
        #endregion
    }
}
