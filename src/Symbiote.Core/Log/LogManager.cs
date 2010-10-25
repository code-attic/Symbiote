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

using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Log.Impl;

namespace Symbiote.Core.Log
{
    public static class LogManager
    {
        private static object _lock = new object();
        private static ILogProvider _provider;
        public static Dictionary<Type, ILogger> _logs = new Dictionary<Type,ILogger>();
        public static bool Initialized;

        public static void Log<T>(LogLevel level, object message)
        {
            Logger<T>().Log(level, message);
        }

        public static void Log<T>(LogLevel level, object message, Exception exception)
        {
            Logger<T>().Log(level, message, exception);
        }

        public static void Log<T>(LogLevel level, string format, params object[] args)
        {
            Logger<T>().Log(level, format, args);
        }

        public static void Log<T>(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            Logger<T>().Log(level, provider, format, args);
        }

        public static ILogger Logger<T>()
        {
            if(!Initialized)
                return new NullLogger();
            return _provider.GetLoggerForType<T>();
        }

        static LogManager()
        {
            try
            {
                _provider = Assimilate.GetInstanceOf<ILogProvider>();
            }
            catch (Exception e)
            {
                _provider = new NullLogProvider();
            }
        }
    }
}