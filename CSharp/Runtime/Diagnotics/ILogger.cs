
using System;

namespace UselessFrame.Runtime.Diagnotics
{
    /// <summary>
    /// Log辅助器
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="content">信息</param>
        void Debug(params object[] content);

        /// <summary>
        /// 警告信息
        /// </summary>
        /// <param name="content">信息</param>
        void Warning(params object[] content);

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="content">信息</param>
        void Error(params object[] content);

        /// <summary>
        /// 致命错误信息
        /// </summary>
        /// <param name="content">信息</param>
        void Fatal(params object[] content);

        /// <summary>
        /// 异常错误
        /// </summary>
        /// <param name="e">异常</param>
        void Exception(Exception e);
    }
}
