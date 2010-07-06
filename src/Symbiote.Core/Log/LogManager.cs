using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Log.Impl;

namespace Symbiote.Core.Log
{
    public static class LogManager
    {
        private static object _lock = new object();
        private static ILogProvider _provider;
        public static Dictionary<Type, ILogger> _logs = new Dictionary<Type,ILogger>();
        public static bool Initialized;

        public static void Log<T>(LogLevel level, object message)
        {
            Logger<T>().Log(level, message);
        }

        public static void Log<T>(LogLevel level, object message, Exception exception)
        {
            Logger<T>().Log(level, message, exception);
        }

        public static void Log<T>(LogLevel level, string format, params object[] args)
        {
            Logger<T>().Log(level, format, args);
        }

        public static void Log<T>(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            Logger<T>().Log(level, provider, format, args);
        }

        public static ILogger Logger<T>()
        {
            if(!Initialized)
                return new NullLogger();
            return _provider.GetLoggerForType<T>();
        }

        static LogManager()
        {
            try
            {
                _provider = ServiceLocator.Current.GetInstance<ILogProvider>();
            }
            catch (Exception e)
            {
                _provider = new NullLogProvider();
            }
        }
    }
}