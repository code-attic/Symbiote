using System;
using System.Collections.Generic;
using log4net;
using Symbiote.Core.Impl.Log;
using Symbiote.Core.Impl.Log.Impl;

namespace Symbiote.Log4Net.Impl
{
    public class Log4NetLogger : ILogger
    {
        private ILog _log;
        
        private Dictionary<LogLevel, Action<ILog, object>> _logMessage = new Dictionary<LogLevel, Action<ILog, object>>()
                                                                             {
                                                                                 {LogLevel.Debug, (l,m) => l.Debug(m)},
                                                                                 {LogLevel.Info, (l,m) => l.Info(m)},
                                                                                 {LogLevel.Warn, (l,m) => l.Warn(m)},
                                                                                 {LogLevel.Error, (l,m) => l.Error(m)},
                                                                                 {LogLevel.Fatal, (l,m) => l.Fatal(m)},
                                                                             };

        private Dictionary<LogLevel, Action<ILog, object, Exception>> _logException = new Dictionary<LogLevel, Action<ILog, object, Exception>>()
                                                                                          {
                                                                                              {LogLevel.Debug, (l,m,x) => l.Debug(m,x)},
                                                                                              {LogLevel.Info, (l,m,x) => l.Info(m,x)},
                                                                                              {LogLevel.Warn, (l,m,x) => l.Warn(m,x)},
                                                                                              {LogLevel.Error, (l,m,x) => l.Error(m,x)},
                                                                                              {LogLevel.Fatal, (l,m,x) => l.Fatal(m,x)},
                                                                                          };

        private Dictionary<LogLevel, Action<ILog, string, object[]>> _logFormat = new Dictionary<LogLevel, Action<ILog, string, object[]>>()
                                                                                      {
                                                                                          {LogLevel.Debug, (l,m,a) => l.DebugFormat(m,a)},
                                                                                          {LogLevel.Info, (l,m,a) => l.InfoFormat(m,a)},
                                                                                          {LogLevel.Warn, (l,m,a) => l.WarnFormat(m,a)},
                                                                                          {LogLevel.Error, (l,m,a) => l.ErrorFormat(m,a)},
                                                                                          {LogLevel.Fatal, (l,m,a) => l.FatalFormat(m,a)},
                                                                                      };

        private Dictionary<LogLevel, Action<ILog, IFormatProvider, string, object[]>> _logFormatProvider = new Dictionary<LogLevel, Action<ILog, IFormatProvider, string, object[]>>()
                                                                                                               {
                                                                                                                   {LogLevel.Debug, (l,f,m,a) => l.DebugFormat(f,m,a)},
                                                                                                                   {LogLevel.Info, (l,f,m,a) => l.InfoFormat(f,m,a)},
                                                                                                                   {LogLevel.Warn, (l,f,m,a) => l.WarnFormat(f,m,a)},
                                                                                                                   {LogLevel.Error, (l,f,m,a) => l.ErrorFormat(f,m,a)},
                                                                                                                   {LogLevel.Fatal, (l,f,m,a) => l.FatalFormat(f,m,a)},
                                                                                                               };


        public void Log(LogLevel level, object message)
        {
            _logMessage[level](_log, message);
        }

        public void Log(LogLevel level, object message, Exception exception)
        {
            _logException[level](_log, message, exception);
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            _logFormat[level](_log, format, args);
        }

        public void Log(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            _logFormatProvider[level](_log, provider, format, args);
        }

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }
    }
}