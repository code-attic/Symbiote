using System;
using log4net.Appender;

namespace Symbiote.Log4Net
{
    public class ColorDefinition
    {
        private ColorMap _map;
        private Action<ColorMap, ColoredConsoleAppender.Colors> _setter;

        public ColorMap IsBlack()
        {
            return _map;
        }

        public ColorMap IsBlue()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Blue);
            return _map;
        }

        public ColorMap IsGreen()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Green);
            return _map;
        }

        public ColorMap IsRed()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Red);
            return _map;
        }

        public ColorMap IsWhite()
        {
            _setter(_map, ColoredConsoleAppender.Colors.White);
            return _map;
        }

        public ColorMap IsYellow()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Yellow);
            return _map;
        }

        public ColorMap IsPurple()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Purple);
            return _map;
        }

        public ColorMap IsCyan()
        {
            _setter(_map, ColoredConsoleAppender.Colors.Cyan);
            return _map;
        }

        public ColorMap IsHighIntensity()
        {
            _setter(_map, ColoredConsoleAppender.Colors.HighIntensity);
            return _map;
        }

        public ColorDefinition(ColorMap map, Action<ColorMap, ColoredConsoleAppender.Colors> setter)
        {
            _map = map;
            _setter = setter;
        }
    }
}