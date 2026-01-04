using System;

namespace XFrame.Modules.Archives
{
    /// <summary>
    /// 存档标记
    /// </summary>
    public class ArchiveAttribute : Attribute
    {
        /// <summary>
        /// 后缀，包含'.'
        /// </summary>
        public string Suffix { get; }

        /// <summary>
        /// 标记一种存档类型
        /// </summary>
        /// <param name="suffix">存档后缀</param>
        public ArchiveAttribute(string suffix)
        {
            if (suffix[0] != '.')
                suffix = "." + suffix;
            Suffix = suffix;
        }
    }
}
