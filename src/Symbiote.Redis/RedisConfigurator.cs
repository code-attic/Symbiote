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
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis
{
    public class RedisConfigurator
    {
        public RedisConfiguration Configuration { get; set; }

        public RedisConfigurator AddServer( string host, int port )
        {
            var server = new RedisHost( host, port );
            Configuration.Hosts[host] = server;
            return this;
        }

        public RedisConfigurator AddServer( string host )
        {
            var server = new RedisHost {Host = host};
            Configuration.Hosts[host] = server;
            return this;
        }

        public RedisConfigurator AddLocalServer()
        {
            var server = new RedisHost();
            Configuration.Hosts["local"] = server;
            return this;
        }

        public RedisConfigurator LimitPoolConnections( int limit )
        {
            Configuration.ConnectionLimit = limit;
            return this;
        }

        public RedisConfigurator Password( string password )
        {
            Configuration.Password = password;
            return this;
        }

        public RedisConfigurator Retries( int retries )
        {
            Configuration.RetryCount = retries;
            return this;
        }

        public RedisConfigurator RetryTimeout( int miliseconds )
        {
            Configuration.RetryTimeout = miliseconds;
            return this;
        }

        public RedisConfigurator SendTimeout( int miliseconds )
        {
            Configuration.SendTimeout = miliseconds;
            return this;
        }

        public RedisConfigurator()
        {
            Configuration = new RedisConfiguration();
        }
        public RedisConfigurator(RedisConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}