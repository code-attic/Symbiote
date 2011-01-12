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

using System.Collections.Concurrent;

namespace Symbiote.Redis.Impl.Config
{
    public class RedisConfiguration
    {
        public ConcurrentDictionary<string, RedisHost> Hosts { get; set; }
        public int RetryTimeout { get; set; }
        public int RetryCount { get; set; }
        public int SendTimeout { get; set; }
        public string Password { get; set; }
        public int ConnectionLimit { get; set; }

        public RedisConfiguration()
        {
            Hosts = new ConcurrentDictionary<string, RedisHost>();
            ConnectionLimit = 10;
            SendTimeout = 30;
            RetryTimeout = 30;
            RetryCount = 5;
        }
    }
}