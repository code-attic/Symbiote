using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Redis;

namespace Redis.Tests
{
    public class with_redis_client: with_assimilation
    {
        protected static IRedisClient client { get; set; }

        private Establish context = () =>
                                    {
                                        Assimilate.Assimilation.Redis( x => x.AddServer("10.15.198.71").LimitPoolConnections( 1 ) );
                                        //Assimilate.Assimilation.Redis( x => x.AddServer("127.0.0.1").LimitPoolConnections( 1 ) );
                                        client = Assimilate.GetInstanceOf<IRedisClient>();
                                    };

    }
}
