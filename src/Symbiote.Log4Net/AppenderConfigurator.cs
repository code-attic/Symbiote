using System;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace Symbiote.Log4Net
{
    public abstract class AppenderConfigurator<TAppender, TConfiguration>
        where TAppender : AppenderSkeleton, new()
        where TConfiguration : AppenderConfigurator<TAppender, TConfiguration>
    {
        protected TAppender _appender;

        internal TAppender Appender { get { return _appender; } }
        internal void Activate() { _appender.ActivateOptions(); }

        public TConfiguration MessageLayout(Action<Pattern> spec)
        {
            var layout = new Pattern();
            spec(layout);
            var pattern = new PatternLayout(layout.ToString());
            pattern.ActivateOptions();
            _appender.Layout = pattern;
            return this as TConfiguration;
        }

        public TConfiguration Fatal()
        {
            _appender.Threshold = Level.Fatal;
            return this as TConfiguration;
        }

        public TConfiguration Error()
        {
            _appender.Threshold = Level.Error;
            return this as TConfiguration;
        }

        public TConfiguration Warning()
        {
            _appender.Threshold = Level.Warn;
            return this as TConfiguration;
        }

        public TConfiguration Debug()
        {
            _appender.Threshold = Level.Debug;
            return this as TConfiguration;
        }

        public TConfiguration Info()
        {
            _appender.Threshold = Level.Info;
            return this as TConfiguration;
        }

        protected abstract void Initialize();

        public AppenderConfigurator()
        {
            _appender = new TAppender();
            Initialize();
        }
    }
}