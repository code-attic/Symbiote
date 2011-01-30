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
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class ConnectionPool
        : IConnectionPool
    {
        protected IRiakConfiguration Configuration { get; set; }
        protected ConcurrentQueue<IProtoBufConnection> AvailableConnections { get; set; }
        protected ConcurrentDictionary<IProtoBufConnection, IProtoBufConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnectionHandle Acquire()
        {
            IProtoBufConnection connection = null;
            while ( connection == null )
            {
                if ( AvailableConnections.TryDequeue( out connection ) ) 
                    continue;
                if ( ReservedConnections.Count + AvailableConnections.Count < Configuration.ConnectionLimit )
                {
                    connection = ConnectionFactory.GetConnection();
                }
            }
            try
            {
                ReservedConnections[connection] = connection;
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
            return new PooledConnectionHandle( this, connection );
        }

        public void Release( IProtoBufConnection connection )
        {
            IProtoBufConnection removed;
            ReservedConnections.TryRemove( connection, out removed );
            AvailableConnections.Enqueue( connection );
        }

        public ConnectionPool( IRiakConfiguration configuration, IConnectionFactory connectionFactory )
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new ConcurrentQueue<IProtoBufConnection>();
            ReservedConnections = new ConcurrentDictionary<IProtoBufConnection, IProtoBufConnection>();
        }
    }
}