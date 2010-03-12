using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;

namespace Symbiote.Telepathy.Appender
{
    public class LoggingMessage
    {
        public virtual DateTime LoggedOn { get; set; }

        public virtual string User { get; set; }

        public virtual string LoggerName { get; set; }

        public virtual LocationInfo LocationInfo { get; set; }

        public virtual string Body { get; set; }

        public virtual string Domain { get; set; }

        public virtual Exception MessageException { get; set; }

        public virtual object ExceptionOccurred { get; set; }

        public virtual string Identity { get; set; }

        public virtual Level Level { get; set; }

        public LoggingMessage(LoggingEvent loggingEvent)
        {
            Domain = loggingEvent.Domain;
            MessageException = loggingEvent.ExceptionObject;
            ExceptionOccurred = MessageException != null;
            Identity = loggingEvent.Identity;
            Level = loggingEvent.Level;
            LocationInfo = loggingEvent.LocationInformation;
            LoggerName = loggingEvent.LoggerName;
            Body = loggingEvent.RenderedMessage;
            LoggedOn = loggingEvent.TimeStamp;
            User = loggingEvent.UserName;
        }
    }
}
