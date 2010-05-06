using System;

namespace Symbiote.Core.Log.Impl
{
    public class NullLogger : ILogger
    {
        public void Log(LogLevel level, object message)
        {
            // do nothing
        }

        public void Log(LogLevel level, object message, Exception exception)
        {
            // do nothing
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            // do nothing
        }

        public void Log(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            // do nothing
        }
    }
}