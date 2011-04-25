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
using Symbiote.Core.Collections;
using Symbiote.Core.Log.Impl;

namespace Symbiote.Core.Log
{
    public interface ILogManager
    {
        void Log<T>( LogLevel level, object message );
        void Log<T>( LogLevel level, object message, Exception exception );
        void Log<T>( LogLevel level, string format, params object[] args );
        void Log<T>( LogLevel level, IFormatProvider provider, string format, params object[] args );
        void Initialize();
    }

    public class LogManager : ILogManager
    {
        private object Lock { get; set; }
        public ILogProvider Provider;
        public ExclusiveConcurrentDictionary<Type, ILogger> Loggers { get; set; }
        public bool Initialized;

        public void Log<T>( LogLevel level, object message )
        {
            GetLoggerFor<T>().Log( level, message );
        }

        public void Log<T>( LogLevel level, object message, Exception exception )
        {
            GetLoggerFor<T>().Log( level, message, exception );
        }

        public void Log<T>( LogLevel level, string format, params object[] args )
        {
            GetLoggerFor<T>().Log( level, format, args );
        }

        public void Log<T>( LogLevel level, IFormatProvider provider, string format, params object[] args )
        {
            GetLoggerFor<T>().Log( level, provider, format, args );
        }

        public ILogger GetLoggerFor<T>()
        {
            var type = typeof(T);
            return Loggers.ReadOrWrite( type, () => Provider.GetLoggerForType<T>() );
        }

        public void Initialize()
        {
            lock( Lock )
            {
                if( Initialized != true )
                {
                    Initialized = true;
                    Provider = Assimilate.GetInstanceOf<ILogProvider>();
                    Loggers = new ExclusiveConcurrentDictionary<Type, ILogger>();
                }
            }
        }

        public LogManager()
        {
            Lock = new object();
            Loggers = new ExclusiveConcurrentDictionary<Type, ILogger>();
            try
            {
                Provider = Assimilate.GetInstanceOf<ILogProvider>();
            }
            catch ( Exception e )
            {
                Provider = new NullLogProvider();
            }
        }
    }
}