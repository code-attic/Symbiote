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
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Impl.Log.Impl
{
    public class TestLogger : ILogger
    {
        public List<string> Content { get; set; }

        public void Log(LogLevel level, object message)
        {
            Content.Add("{0} - {1}".AsFormat(level, message));
        }

        public void Log(LogLevel level, object message, Exception exception)
        {
            Content.Add("{0} - {1}".AsFormat(level, message));
        }

        public void Log(LogLevel level, string format, params object[] args)
        {
            Content.Add("{0} - {1}".AsFormat(level, format.AsFormat(args)));
        }

        public void Log(LogLevel level, IFormatProvider provider, string format, params object[] args)
        {
            Content.Add("{0} - {1}".AsFormat(level, format.AsFormat(args)));
        }

        public TestLogger()
        {
            Content = new List<string>();
        }
    }
}