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
using System.Collections.Concurrent;
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis.Impl.Connection
{
    public class ConnectionPool
        : IConnectionPool
    {
        protected RedisConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IConnection> AvailableConnections { get; set; }
        protected ConcurrentDictionary<IConnection, IConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnection Acquire()
        {
            IConnection connection = null;
            while ( connection == null )
            {
                if ( !AvailableConnections.TryDequeue( out connection ) )
                {
                    if ( ReservedConnections.Count < Configuration.ConnectionLimit )
                    {
                        connection = ConnectionFactory.GetConnection();
                    }
                }
            }
            try
            {
                ReservedConnections[connection] = connection;
            }
            catch ( Exception e )
            {
                //TODO: put real error handling here
                Console.WriteLine( e );
            }
            return connection;
        }

        public void Release( IConnection connection )
        {
            IConnection removed;
            ReservedConnections.TryRemove( connection, out removed );
            AvailableConnections.Enqueue( connection );
        }

        public ConnectionPool( RedisConfiguration configuration, IConnectionFactory connectionFactory )
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IConnection>();
            ReservedConnections = new ConcurrentDictionary<IConnection, IConnection>();
        }
    }
}