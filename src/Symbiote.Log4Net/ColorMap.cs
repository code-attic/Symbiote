/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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