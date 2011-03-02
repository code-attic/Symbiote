using System;

namespace Symbiote.Core.Log.Impl
{
    public interface ILogger
    {
        void Log(LogLevel level, object message);
        void Log(LogLevel level, object message, Exception exception);
        void Log(LogLevel level, string format, params object[] args);
        void Log(LogLevel level, IFormatProvider provider, string format, params object[] args);
    }
}
