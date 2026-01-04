using UnityEngine;
using System.Collections.Generic;

namespace UnityXFrame.Core.Diagnotics
{
    public partial class UnityLogger
    {
        private class Formater
        {
            private const int MAX_LENGTH = 4096;
            private Dictionary<string, Color> m_Colors;

            public Formater()
            {
                m_Colors = new Dictionary<string, Color>();
            }

            public void Register(string name, Color color)
            {
                m_Colors.Add(name, color);
            }

            public bool Format(string name, string content, out string result)
            {
                if (m_Colors.TryGetValue(name, out Color color))
                {
                    InnerFormat(color, name, content, out result);
                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(name) && m_Colors.TryGetValue("Default", out color))
                    {
                        InnerFormat(color, "Default", content, out result);
                        return true;
                    }
                    else
                    {
                        result = content;
                        return false;
                    }
                }
            }

            private void InnerFormat(Color color, string name, string content, out string result)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                if (content.Length > MAX_LENGTH)
                    content = content.Substring(0, MAX_LENGTH);
                if (!string.IsNullOrEmpty(name))
                    content = $"<color=#00FFFF> [{name}] </color> <color=#{colorHex}> {content} </color>";
                else
                    content = $"<color=#{colorHex}> {content} </color> ";
                content = content.Replace("\n", $"</color>\n<color=#{colorHex}>");
                result = content;
            }
        }
    }
}
