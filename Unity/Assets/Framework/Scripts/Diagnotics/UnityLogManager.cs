
using System;
using UnityEngine;
using UselessFrame.NewRuntime;

namespace UnityXFrame.Core.Diagnotics
{
    internal class UnityLogManager : LogManager
    {
        public UnityLogManager(UselessFrame.NewRuntime.ILogger[] loggers) : base(loggers)
        {
        }

        [HideInCallstack]
        public override void Debug(params object[] content)
        {
            foreach (UselessFrame.NewRuntime.ILogger logger in m_Loggers)
                logger.Debug(content);
        }

        [HideInCallstack]
        public override void Warning(params object[] content)
        {
            foreach (UselessFrame.NewRuntime.ILogger logger in m_Loggers)
                logger.Warning(content);
        }

        [HideInCallstack]
        public override void Error(params object[] content)
        {
            foreach (UselessFrame.NewRuntime.ILogger logger in m_Loggers)
                logger.Error(content);
        }

        [HideInCallstack]
        public override void Fatal(params object[] content)
        {
            foreach (UselessFrame.NewRuntime.ILogger logger in m_Loggers)
                logger.Fatal(content);
        }

        [HideInCallstack]
        public override void Exception(Exception e)
        {
            foreach (UselessFrame.NewRuntime.ILogger logger in m_Loggers)
                logger.Exception(e);
        }
    }
}
