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

namespace Symbiote.Redis.Impl.Config
{
    public class RedisHost
    {
        public string Host { get; set; }
        public int Port { get; private set; }

        public void Init()
        {
            Host = "localhost";
            Port = 6379;
        }

        public RedisHost()
        {
            Init();
        }

        public RedisHost(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}