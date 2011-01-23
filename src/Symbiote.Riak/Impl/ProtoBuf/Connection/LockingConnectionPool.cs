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
using System.Threading;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class LockingConnectionPool
        : IConnectionPool
    {
        protected IRiakConfiguration Configuration { get; set; }
        protected ReaderWriterLockSlim AvailableLock { get; set; }
        protected ReaderWriterLockSlim ReservedLock { get; set; }
        protected Queue<IProtoBufConnection> AvailableConnections { get; set; }
        protected HashSet<IProtoBufConnection> ReservedConnections { get; set; }
        protected IConnectionFactory ConnectionFactory { get; set; }

        public IConnectionHandle Acquire()
        {
            IProtoBufConnection connection = null;
            do
            {
                AvailableLock.EnterUpgradeableReadLock();
                if (connection == null && AvailableConnections.Count > 0)
                {
                    AvailableLock.EnterWriteLock();
                    connection = AvailableConnections.Dequeue();
                    AvailableLock.ExitWriteLock();
                }
                AvailableLock.ExitUpgradeableReadLock();
                if (connection != null)
                {
                    ReservedLock.EnterWriteLock();
                    ReservedConnections.Add(connection);
                    ReservedLock.ExitWriteLock();
                }
            } while (connection == null);
            return new PooledConnectionHandle( this, connection );
        }

        public void Release( IProtoBufConnection connection )
        {
            ReservedLock.EnterWriteLock();
            AvailableLock.EnterWriteLock();
            ReservedConnections.Remove(connection);
            AvailableConnections.Enqueue(connection);
            AvailableLock.ExitWriteLock();
            ReservedLock.ExitWriteLock();
        }

        public LockingConnectionPool( IRiakConfiguration configuration, IConnectionFactory connectionFactory )
        {
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableLock = new ReaderWriterLockSlim();
            ReservedLock = new ReaderWriterLockSlim();
            Configuration = configuration;
            ConnectionFactory = connectionFactory;
            AvailableConnections = new Queue<IProtoBufConnection>();
            ReservedConnections = new HashSet<IProtoBufConnection>();
            while (AvailableConnections.Count < Configuration.ConnectionLimit)
            {
                AvailableConnections.Enqueue(ConnectionFactory.GetConnection());
            }
        }
    }
}