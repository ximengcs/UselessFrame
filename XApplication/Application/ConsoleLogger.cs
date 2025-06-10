using UselessFrame.NewRuntime;

namespace Core.Application
{
    public partial class ConsoleLogger : ILogger
    {
        private Formater m_Formater;

        public ConsoleLogger()
        {
            m_Formater = new Formater();
            Console.CursorVisible = false;
        }

        public void Register(string name, ConsoleColor color)
        {
            m_Formater.Register(name, color);
        }

        public void Debug(params object[] content)
        {
            lock (m_Formater)
            {
                InnerFormat(out string head, out string result, content);
                if (!string.IsNullOrEmpty(head))
                {
                    m_Formater.Head();
                    Console.Write($"[{head}]".PadRight(8, ' '));
                    m_Formater.End();
                }

                m_Formater.Begin(head);
                Console.WriteLine(result);
                m_Formater.End();
            }
        }

        public void Error(params object[] content)
        {
            lock (m_Formater)
            {
                m_Formater.Error();
                InnerFormat(out string head, out string result, content);
                Console.WriteLine(result);
                m_Formater.End();
            }
        }

        public void Exception(Exception e)
        {
            lock (m_Formater)
            {
                m_Formater.Fatal();
                Console.WriteLine(e);
                m_Formater.End();
            }
        }

        public void Fatal(params object[] content)
        {
            lock (m_Formater)
            {
                m_Formater.Fatal();
                InnerFormat(out string head, out string result, content);
                Console.WriteLine(result);
                m_Formater.End();
            }
        }

        public void Warning(params object[] content)
        {
            lock (m_Formater)
            {
                m_Formater.Warning();
                InnerFormat(out string head, out string result, content);
                Console.WriteLine(result);
                m_Formater.End();
            }
        }

        private bool InnerFormat(out string head, out string result, params object[] content)
        {
            if (content.Length > 1)
            {
                head = $"{content[0]}";
                if (content.Length > 2)
                {
                    object[] contentList = new object[content.Length - 2];
                    for (int i = 0; i < contentList.Length; i++)
                        contentList[i] = content[i + 2];
                    result = string.Concat($"[{Thread.CurrentThread.ManagedThreadId,5}]", string.Format((string)content[1], contentList));
                }
                else
                {
                    result = $"[{Thread.CurrentThread.ManagedThreadId,5}]{content[1]}";
                }
                return true;
            }
            else
            {
                result = string.Concat($"[{Thread.CurrentThread.ManagedThreadId,5}]", content[0]);
                head = null;
                return true;
            }
        }
    }
}
