
using System;
using System.Collections.Generic;

namespace UselessFrame.Runtime.Diagnotics
{
    public class LogSystem : ILogSystem
    {
        private bool _power;
        private IFrameCore _core;
        private List<ILogger> _loggers;

        public bool Power
        {
            get => _power;
            set => _power = value;
        }

        public LogSystem(IFrameCore core)
        {
            _core = core;
            _power = true;
            _loggers = new List<ILogger>();
        }

        public void AddLogger<T>() where T : ILogger
        {
            _loggers.Add(InnerAddLogger(typeof(T)));
        }

        public void RemoveLogger<T>() where T : ILogger
        {
            T logger = GetLogger<T>();
            if (logger != null)
                _loggers.Remove(logger);
        }

        public T GetLogger<T>() where T : ILogger
        {
            foreach (ILogger logger in _loggers)
            {
                if (logger.GetType() == typeof(T))
                    return (T)logger;
            }
            return default;
        }

        public void Debug(params object[] content)
        {
            if (!_power)
                return;
            foreach (ILogger logger in _loggers)
                logger.Debug(content);
        }

        public void Warning(params object[] content)
        {
            if (!_power)
                return;
            foreach (ILogger logger in _loggers)
                logger.Warning(content);
        }

        public void Error(params object[] content)
        {
            if (!_power)
                return;
            foreach (ILogger logger in _loggers)
                logger.Error(content);
        }

        public void Fatal(params object[] content)
        {
            if (!_power)
                return;
            foreach (ILogger logger in _loggers)
                logger.Fatal(content);
        }

        public void Exception(Exception e)
        {
            if (!_power)
                return;
            foreach (ILogger logger in _loggers)
                logger.Exception(e);
        }

        private ILogger InnerAddLogger(Type type)
        {
            ILogger logger = (ILogger)_core.TypeSystem.CreateInstance(type);
            _loggers.Add(logger);
            return logger;
        }
    }
}
