using System;
using System.Threading;
using UnityEngine;

namespace UnityXFrame.Core.Diagnotics
{
    public partial class UnityLogger : UselessFrame.NewRuntime.ILogger, YooAsset.ILogger
    {
        private bool m_MustRegister;
        private Formater m_Formater;

        public UnityLogger()
        {
            m_MustRegister = false;
            m_Formater = new Formater();
        }

        public void Register(string name, Color color)
        {
            m_Formater.Register(name, color);
        }

        void YooAsset.ILogger.Log(string message)
        {
            Debug(LogSort.Resource, message);
        }

        void YooAsset.ILogger.Warning(string message)
        {
            Warning(LogSort.Resource, message);
        }

        void YooAsset.ILogger.Error(string message)
        {
            Error(LogSort.Resource, message);
        }

        [HideInCallstack]
        public void Debug(params object[] content)
        {
            if (InnerFormat(out string result, content) || !m_MustRegister)
            {
                UnityEngine.Debug.Log(result);
            }
        }

        [HideInCallstack]
        public void Error(params object[] content)
        {
            if (InnerFormat(out string result, content) || !m_MustRegister)
            {
                UnityEngine.Debug.LogError(result);
            }
        }

        [HideInCallstack]
        public void Fatal(params object[] content)
        {
            if (InnerFormat(out string result, content) || !m_MustRegister)
            {
                UnityEngine.Debug.LogError(result);
            }
        }

        [HideInCallstack]
        public void Warning(params object[] content)
        {
            if (InnerFormat(out string result, content) || !m_MustRegister)
            {
                UnityEngine.Debug.LogWarning(result);
            }
        }

        private bool InnerFormat(out string result, params object[] content)
        {
            if (content.Length > 1)
            {
                string realContent;
                if (content.Length > 2)
                {
                    object[] contentList = new object[content.Length - 2];
                    for (int i = 0; i < contentList.Length; i++)
                        contentList[i] = content[i + 2];
                    realContent = string.Format((string)content[1], contentList);
                }
                else
                {
                    realContent = content[1].ToString();
                }
                bool success = m_Formater.Format(content[0].ToString(), realContent, out result);
                result = $"[{Thread.CurrentThread.ManagedThreadId,5}]" + result;
                if (success)
                    return true;
                else
                    return false;
            }
            else
            {
                if (content.Length == 1)
                    m_Formater.Format(string.Empty, content[0].ToString(), out result);
                else
                    result = string.Concat(content);
                result = $"[{Thread.CurrentThread.ManagedThreadId,5}]" + result;
                return true;
            }
        }

        [HideInCallstack]
        public void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}
