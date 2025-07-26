using System;
using System.IO;

namespace UselessFrame.NewRuntime.Utilities
{
    /// <summary>
    /// 路径常用方法
    /// </summary>
    public class PathUtility
    {
        /// <summary>
        /// 将路径 \\ 格式化为 /
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>格式化后的路径</returns>
        public static string Format1(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// 将路径 / 格式化为 \\
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>格式化后的路径</returns>
        public static string Format2(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// 移除路径中的回车符 \n \r
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>格式化后的路径</returns>
        public static string RemoveEnterChar(string path)
        {
            path = path.Replace("\n", "");
            path = path.Replace("\r", "");
            return path;
        }

        /// <summary>
        /// 检查文件路径层级
        /// </summary>
        /// <param name="fullPath">文件路径</param>
        /// <param name="thisName">当前路径</param>
        /// <param name="suplusName">剩余路径</param>
        /// <returns>层级数</returns>
        public static int CheckFileName(string fullPath, out string thisName, out string suplusName)
        {
            fullPath = fullPath.Replace("/", "\\");
            string[] paths = fullPath.Split('\\');
            if (paths.Length == 0)
            {
                thisName = default;
                suplusName = default;
                return default;
            }
            else if (paths.Length == 1)
            {
                thisName = paths[0];
                suplusName = default;
                return 1;
            }
            else
            {
                thisName = paths[0];
                string[] supluses = new string[paths.Length - 1];
                Array.Copy(paths, 1, supluses, 0, paths.Length - 1);
                suplusName = Path.Combine(supluses);
                return 2;
            }
        }
    }
}
