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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Symbiote.Core.Extensions;

namespace Symbiote.Eidetic.Config
{
    public class DefaultMemcachedConfiguration : MemcachedClientConfiguration
    {
        public IList<IPEndPoint> ServerEndpoints
        {
            get { return Servers; }
        }

        public int MinPoolSize
        {
            get { return SocketPool.MinPoolSize; }
            set { SocketPool.MinPoolSize = value; }
        }

        public int MaxPoolSize
        {
            get { return SocketPool.MaxPoolSize; }
            set { SocketPool.MaxPoolSize = value; }
        }

        public TimeSpan TimeOut
        {
            get { return SocketPool.ConnectionTimeout; }
            set { SocketPool.ConnectionTimeout = value; }
        }

        public TimeSpan DeadTimeout
        {
            get { return SocketPool.DeadTimeout; }
            set { SocketPool.DeadTimeout = value; }
        }

        public void Initialize()
        {
            var section =
                ConfigurationManager.GetSection( "memcached" ) as IMemcachedConfig ?? new MemcachedDefaults();

            MinPoolSize = section.MinPoolSize;
            MaxPoolSize = section.MaxPoolSize;
            TimeOut = TimeSpan.FromSeconds( section.Timeout );
            DeadTimeout = TimeSpan.FromSeconds( section.DeadTimeout );

            section
                .Servers
                .ForEach( server =>
                          ServerEndpoints.Add( GetEndPoint( server.Address, server.Port ) )
                );
        }

        private IPEndPoint GetEndPoint( string address, int port )
        {
            var host = new IPHostEntry();
            host.HostName = address;
            var ipAddress = Dns.GetHostAddresses( address ).First();
            return new IPEndPoint( ipAddress, port );
        }

        public DefaultMemcachedConfiguration()
        {
            NodeLocator = typeof( DefaultNodeLocator );
            KeyTransformer = new Base64KeyTransformer();
            Transcoder = new DefaultTranscoder();
            Initialize();
        }
    }
}