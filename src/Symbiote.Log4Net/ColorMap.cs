using log4net.Appender;
using log4net.Core;

namespace Symbiote.Log4Net
{
    public class ColorMap
    {

        internal ColoredConsoleAppender.Colors TextColor { get; set;}
        internal ColoredConsoleAppender.Colors BackGroundColor { get; set;}
        internal Level Level { get; set; }

        private ColorConsoleConfigurator _config;
        private ColorDefinition _text;
        private ColorDefinition _background;

        public ColorDefinition Text
        {
            get
            {
                return _text;
            }
        }

        public ColorDefinition BackGround
        {
            get
            {
                return _background;
            }
        }

        public ColorConsoleConfigurator ForAllOutput()
        {
            Level = Level.All;
            return Apply();
        }

        public ColorConsoleConfigurator ForInfo()
        {
            Level = Level.Info;
            return Apply();
        }

        public ColorConsoleConfigurator ForDebug()
        {
            Level = Level.Debug;
            return Apply();
        }

        public ColorConsoleConfigurator ForWarning()
        {
            Level = Level.Warn;
            return Apply();
        }

        public ColorConsoleConfigurator ForError()
        {
            Level = Level.Error;
            return Apply();
        }

        public ColorConsoleConfigurator ForFatal()
        {
            Level = Level.Fatal;
            return Apply();
        }

        private ColorConsoleConfigurator Apply()
        {
            _config.ApplyColorMap(this);
            return _config;
        }

        public ColorMap(ColorConsoleConfigurator config)
        {
            _config = config;
            _text = new ColorDefinition(this, (m,c) => TextColor = c);
            _background = new ColorDefinition(this, (m, c) => BackGroundColor = c);
        }
    }
}