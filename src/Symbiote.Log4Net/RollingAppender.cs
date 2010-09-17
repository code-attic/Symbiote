using System;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace Symbiote.Log4Net
{
    public class RollingConfiguration : AppenderConfigurator<RollingFileAppender, RollingConfiguration>
    {
        public RollingConfiguration FileName(string file)
        {
            _appender.File = file;
            return this;
        }

        public RollingConfiguration OverwriteLogFiles()
        {
            _appender.AppendToFile = false;
            return this;
        }

        public RollingConfiguration RollOverWhenLogIs(int byteCount)
        {
            _appender.MaxFileSize = byteCount;
            return this;
        }

        public RollingConfiguration LimitLogFileCountTo(int limit)
        {
            _appender.MaxSizeRollBackups = limit;
            return this;
        }

        protected override void Initialize()
        {
            _appender.AppendToFile = true;
            _appender.Encoding = Encoding.UTF8;
            _appender.File = "application.log";
            _appender.ImmediateFlush = true;
            _appender.LockingModel = new FileAppender.MinimalLock();
            _appender.Name = "rollingfile";
            _appender.Threshold = Level.All;
            _appender.CountDirection = 1;
            _appender.DatePattern = "MM_dd_yyyy";
            _appender.MaxFileSize = (1024 ^ 3) * 2;
            _appender.MaxSizeRollBackups = 10;
            _appender.StaticLogFileName = false;
        }
    }
}