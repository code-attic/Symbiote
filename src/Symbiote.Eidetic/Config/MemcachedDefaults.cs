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

using System.Collections.Generic;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedDefaults : IMemcachedConfig
    {
        private int _minPoolSize = 10;
        private int _maxPoolSize = 100;
        private int _timeout = 10;
        private int _deadTimeout = 30;

        public int MinPoolSize
        {
            get { return _minPoolSize; }
            set { _minPoolSize = value; }
        }
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set { _maxPoolSize = value; }
        }
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public int DeadTimeout
        {
            get { return _deadTimeout; }
            set { _deadTimeout = value; }
        }
        public IEnumerable<MemcachedServer> Servers
        {
            get
            {
                yield return new MemcachedServer()
                             {
                                 Address = "localhost",
                                 Port = 11211
                             };
            }   
        }
    }
}