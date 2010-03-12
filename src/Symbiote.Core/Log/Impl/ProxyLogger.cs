using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Log.Impl
{
    public class ProxyLogger<T> : ILogger<T>
    {
        public void Log(LogLevel level, object message)
        {
            LogManager.Log<T>(level, message);
        }

        public void Log(LogLevel level, object message, Exception exception)
        {
            LogManager.Log<T>(level, message, exception);
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            LogManager.Log<T>(level, format, args);
        }

        public void Log(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            LogManager.Log<T>(level, provider, format, args);
        }
    }
}
