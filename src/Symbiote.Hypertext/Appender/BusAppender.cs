using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using StructureMap;
using Symbiote.Telepathy.Appender;

namespace Symbiote.Jackalope.Appender
{
    public class BusAppender : AppenderSkeleton
    {
        private IBus _bus;
        private string _exchangeName;

        public string ExchangeName
        {
            get { return _exchangeName; }
            set { _exchangeName = value; }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            _bus.Send(_exchangeName, new LoggingMessage(loggingEvent), loggingEvent.LoggerName);
        }

        public BusAppender()
        {
            _bus = ObjectFactory.GetInstance<IBus>();
        }
    }
}
