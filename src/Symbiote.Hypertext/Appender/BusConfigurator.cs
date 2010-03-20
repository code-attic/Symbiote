using log4net.Core;
using log4net.Layout;
using Symbiote.Log4Net;

namespace Symbiote.Jackalope.Appender
{
    public class BusConfigurator : AppenderConfigurator<BusAppender,BusConfigurator>
    {
        public BusConfigurator ExchangeName(string exchangeName)
        {
            _appender.ExchangeName = exchangeName;
            return this;
        }

        protected override void Initialize()
        {
            _appender.ActivateOptions();
            _appender.Name = "busAppender";
            _appender.ExchangeName = "log4Net";
            _appender.Threshold = Level.All;
            var layout =
                new PatternLayout(
                    Pattern
                        .New()
                        .Message()
                        .Newline()
                        .ToString()
                    );
            layout.ActivateOptions();
            _appender.Layout = layout;
        }
    }
}