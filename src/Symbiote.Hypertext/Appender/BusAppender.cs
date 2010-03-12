using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using StructureMap;

namespace Symbiote.Jackalope.Appender
{
    public class BusAppender : AppenderSkeleton
    {
        private IBus _bus;

        protected override void Append(LoggingEvent loggingEvent)
        {
            //_bus.Send();
        }

        public BusAppender()
        {
            _bus = ObjectFactory.GetInstance<IBus>();
        }
    }
}
