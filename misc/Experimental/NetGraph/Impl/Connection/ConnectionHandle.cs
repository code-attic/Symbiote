using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Redis;

namespace NetGraph.Impl.Connection
{
    public class ConnectionHandle
        : IDisposable
    {
        public IRedisClient Client { get; set; }

        public static ConnectionHandle Acquire()
        {
            var redisClient = ServiceLocator.Current.GetInstance<IRedisClient>();
            return new ConnectionHandle(redisClient);
        }

        public ConnectionHandle(IRedisClient redisClient)
        {
            Client = redisClient;
        }

        public void Dispose()
        {
            Client = null;
        }
    }
}
