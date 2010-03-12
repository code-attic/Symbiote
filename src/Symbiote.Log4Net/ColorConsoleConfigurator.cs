using System;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace Symbiote.Log4Net
{
    public class ColorConsoleConfigurator : AppenderConfigurator<ColoredConsoleAppender, ColorConsoleConfigurator>
    {
        public ColorMap DefineColor()
        {
            return new ColorMap(this );
        }

        internal void ApplyColorMap(ColorMap map)
        {
            _appender.AddMapping(new ColoredConsoleAppender.LevelColors()
                                     {
                                         ForeColor = map.TextColor,
                                         BackColor = map.BackGroundColor,
                                         Level = map.Level
                                     });
        }

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
                        .Date()
                        .Level()
                        .Message()
                        .ToString()
                    );
            layout.ActivateOptions();
            _appender.Layout = layout;
        }
    }
}