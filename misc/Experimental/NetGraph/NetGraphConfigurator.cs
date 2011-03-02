using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using NetGraph.Impl.Config;
using Symbiote.Redis;
using Symbiote.Redis.Impl.Config;

namespace NetGraph
{
    public class NetGraphConfigurator
    {

        public NetGraphConfiguration Configuration { get; set; }

        public NetGraphConfigurator AddRedisClient(IRedisClient redisClient)
        {
            Configuration.RedisClient = redisClient;
            return this;
        }

        public NetGraphConfigurator()
        {
            Configuration = new NetGraphConfiguration();
            Configuration.RedisClient = ServiceLocator.Current.GetInstance<IRedisClient>();
        }

    }
}
