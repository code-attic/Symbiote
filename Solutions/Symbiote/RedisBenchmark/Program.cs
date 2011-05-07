using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Redis;

namespace RedisBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .UseTestLogAdapter()
                .Redis(x => x.AddServer("10.15.198.71").LimitPoolConnections(1));

            var mgr = new RedisBenchmarkMgr(Assimilate.GetInstanceOf<IRedisClient>());
            mgr.Execute();
            Console.ReadLine();
        }
    }
}
