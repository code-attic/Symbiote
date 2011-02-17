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
using System.Text;
using log4net.Appender;
using log4net.Core;
using Symbiote.Log4Net.Config;

namespace Symbiote.Log4Net.Impl
{
    public class RollingConfiguration : AppenderConfigurator<RollingFileAppender, RollingConfiguration>
    {
        public RollingConfiguration FileName( string file )
        {
            _appender.File = file;
            return this;
        }

        public RollingConfiguration OverwriteLogFiles()
        {
            _appender.AppendToFile = false;
            return this;
        }

        public RollingConfiguration RollOverWhenLogIs( int byteCount )
        {
            _appender.MaxFileSize = byteCount;
            return this;
        }

        public RollingConfiguration LimitLogFileCountTo( int limit )
        {
            _appender.MaxSizeRollBackups = limit;
            return this;
        }

        protected override void Initialize()
        {
            _appender.AppendToFile = true;
            _appender.Encoding = Encoding.UTF8;
            _appender.File = "application.log";
            _appender.ImmediateFlush = true;
            _appender.LockingModel = new FileAppender.MinimalLock();
            _appender.Name = "rollingfile";
            _appender.Threshold = Level.All;
            _appender.CountDirection = 1;
            _appender.DatePattern = "MM_dd_yyyy";
            _appender.MaxFileSize = (1024 ^ 3)*2;
            _appender.MaxSizeRollBackups = 10;
            _appender.StaticLogFileName = false;
        }
    }
}