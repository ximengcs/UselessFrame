
using System;
using System.Collections.Concurrent;

namespace UselessFrame.NewRuntime
{
    public class LogManager : ILogManager
    {
        protected ConcurrentBag<ILogger> m_Loggers;

        public LogManager(ILogger[] loggers)
        {
            if (loggers != null)
                m_Loggers = new ConcurrentBag<ILogger>(loggers);
            else
                m_Loggers = new ConcurrentBag<ILogger>();
        }

        #region Interface
        /// <inheritdoc/>
        public void AddLogger<T>() where T : ILogger
        {
            InnerAddLogger(typeof(T));
        }

        public void AddLogger(ILogger logger)
        {
            m_Loggers.Add(logger);
        }

        public void AddLogger(Type type)
        {
            InnerAddLogger(type);
        }

        /// <inheritdoc/>
        public T GetLogger<T>() where T : ILogger
        {
            foreach (ILogger logger in m_Loggers)
            {
                if (logger.GetType() == typeof(T))
                    return (T)logger;
            }
            return default;
        }

        /// <inheritdoc/>
        public virtual void Debug(params object[] content)
        {
            foreach (ILogger logger in m_Loggers)
                logger.Debug(content);
        }

        /// <inheritdoc/>
        public virtual void Warning(params object[] content)
        {
            foreach (ILogger logger in m_Loggers)
                logger.Warning(content);
        }

        /// <inheritdoc/>
        public virtual void Error(params object[] content)
        {
            foreach (ILogger logger in m_Loggers)
                logger.Error(content);
        }

        /// <inheritdoc/>
        public virtual void Fatal(params object[] content)
        {
            foreach (ILogger logger in m_Loggers)
                logger.Fatal(content);
        }

        /// <inheritdoc/>
        public virtual void Exception(Exception e)
        {
            foreach (ILogger logger in m_Loggers)
                logger.Exception(e);
        }
        #endregion

        private ILogger InnerAddLogger(Type type)
        {
            ILogger logger = X.Type.CreateInstance(type) as ILogger;
            m_Loggers.Add(logger);
            return logger;
        }
    }
}
