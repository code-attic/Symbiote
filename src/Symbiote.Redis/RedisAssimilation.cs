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
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Redis.Impl;
using Symbiote.Redis.Impl.Config;
using Symbiote.Redis.Impl.Connection;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis
{
    public static class RedisAssimilation
    {
        public static IAssimilate Redis(this IAssimilate assimilate, Action<RedisConfigurator> configure)
        {
            var configurator = new RedisConfigurator();
            configure(configurator);

            Assimilate
                .Dependencies(x =>
                                  {
                                      x.For<RedisConfiguration>().Use(configurator.Configuration);
                                      x.For<IRedisConnectionPool>().Use<LockingRedisConnectionPool>().AsSingleton();
                                      x.For<IRedisClient>().Use<RedisClient>();
                                      x.For<ICacheSerializer>().Use<JsonCacheSerializer>();
                                      x.For<IRedisConnectionFactory>().Use<RedisConnectionFactory>();
                                  });
            return assimilate;
        }
    }
}
