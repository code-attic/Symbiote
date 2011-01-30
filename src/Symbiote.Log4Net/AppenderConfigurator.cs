// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace Symbiote.Log4Net
{
    public abstract class AppenderConfigurator<TAppender, TConfiguration>
        where TAppender : AppenderSkeleton, new()
        where TConfiguration : AppenderConfigurator<TAppender, TConfiguration>
    {
        protected TAppender _appender;

        internal TAppender Appender
        {
            get { return _appender; }
        }

        internal void Activate()
        {
            _appender.ActivateOptions();
        }

        public TConfiguration MessageLayout( Action<Pattern> spec )
        {
            var layout = new Pattern();
            spec( layout );
            var pattern = new PatternLayout( layout.ToString() );
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