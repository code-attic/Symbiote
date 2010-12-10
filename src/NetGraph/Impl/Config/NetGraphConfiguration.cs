using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Config
{
    public class NetGraphConfiguration
    {
        public IRedisClient RedisClient { get; set; }
    }
}
