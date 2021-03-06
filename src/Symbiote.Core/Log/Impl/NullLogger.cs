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

namespace Symbiote.Core.Log.Impl
{
    public class NullLogger : ILogger
    {
        public static bool Called { get; set; }

        public void Log( LogLevel level, object message )
        {
            Called = true;
        }

        public void Log( LogLevel level, object message, Exception exception )
        {
            Called = true;
        }

        public void Log( LogLevel level, string format, params object[] args )
        {
            Called = true;
        }

        public void Log( LogLevel level, IFormatProvider provider, string format, params object[] args )
        {
            Called = true;
        }
    }
}