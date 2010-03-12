using System;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace Symbiote.Log4Net
{
    public class ConsoleConfigurator : AppenderConfigurator<ConsoleAppender, ConsoleConfigurator>
    {
        protected override void Initialize()
        {
            _appender.ActivateOptions();
            _appender.Name = "consoleAppender";
            _appender.Target = "Console.Out";
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