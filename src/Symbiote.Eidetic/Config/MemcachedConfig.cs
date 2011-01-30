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
using System.Collections.Generic;
using System.Configuration;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedConfig : ConfigurationSection, IMemcachedConfig
    {
        [ConfigurationProperty( "servers" )]
        public MemcachedServerList ServerList
        {
            get { return this["servers"] as MemcachedServerList; }
            set { this["servers"] = value; }
        }

        #region IMemcachedConfig Members

        public IEnumerable<MemcachedServer> Servers
        {
            get
            {
                foreach( MemcachedServer server in ServerList )
                {
                    yield return server;
                }
            }
        }

        [ConfigurationProperty( "minpoolsize", DefaultValue = 10 )]
        public int MinPoolSize
        {
            get { return (int) this["minpoolsize"]; }
            set { this["minpoolsize"] = value; }
        }

        [ConfigurationProperty( "maxpoolsize", DefaultValue = 100 )]
        public int MaxPoolSize
        {
            get { return (int) this["maxpoolsize"]; }
            set { this["maxpoolsize"] = value; }
        }

        [ConfigurationProperty( "timeout", DefaultValue = 10 )]
        public int Timeout
        {
            get { return (int) this["timeout"]; }
            set { this["timeout"] = value; }
        }

        [ConfigurationProperty( "deadtimeout", DefaultValue = 30 )]
        public int DeadTimeout
        {
            get { return (int) this["deadtimeout"]; }
            set { this["deadtimeout"] = value; }
        }

        #endregion
    }
}