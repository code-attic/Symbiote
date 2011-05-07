using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Redis;

namespace Redis.Tests
{
    public class with_clean_db:with_redis_client
    {
        private Establish context = () =>
                                        {
                                            client.SelectDb(1);
                                            client.FlushDb();
                                        };

    }
}
