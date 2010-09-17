using System;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace Symbiote.Log4Net
{
    public class FileConfigurator : AppenderConfigurator<FileAppender, FileConfigurator>
    {
        public FileConfigurator FileName(string file)
        {
            _appender.File = file;
            return this;
        }

        public FileConfigurator OverwriteLogFiles()
        {
            _appender.AppendToFile = false;
            return this;
        }

        protected override void Initialize()
        {
            _appender.AppendToFile = true;
            _appender.Encoding = Encoding.UTF8;
            _appender.File = "application.log";
            _appender.ImmediateFlush = true;
            _appender.LockingModel = new FileAppender.MinimalLock();
            _appender.Name = "file";
            _appender.Threshold = Level.All;
        }
    }
}