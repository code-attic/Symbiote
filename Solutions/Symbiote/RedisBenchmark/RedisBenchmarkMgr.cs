using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis;

namespace RedisBenchmark
{
    class RedisBenchmarkMgr
    {
        protected IRedisClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the RedisBenchmarkMgr class.
        /// </summary>
        public RedisBenchmarkMgr(IRedisClient client)
        {
            Client = client;
        }

        public void Execute()
        {
            for (int j = 0; j < 10; j++)
            {
                Console.WriteLine("Run {0} ***************".AsFormat(j));
                //int ctOfValInDb = 0;
                int ctOfLoops = 10000;
                Stopwatch watch = Stopwatch.StartNew();
                ExecuteReadWrite(0);
                var FirstExeMs = watch.ElapsedMilliseconds;

                for (int i = 0; i < ctOfLoops; i++)
                {
                    ExecuteReadWrite(i);
                }
                long runMilliseconds = watch.ElapsedMilliseconds;

                Console.WriteLine("Set/Get first execution in {0} ms ({1:0.####} ms/operation)".AsFormat(FirstExeMs, FirstExeMs / (2.0)));
                Console.WriteLine("Set/Get {0} executions in {1} ms ({2:0.####} ms/operation)".AsFormat(ctOfLoops, runMilliseconds, runMilliseconds / (ctOfLoops * 2.0)));
            }
        }
        private void ExecuteReadWrite(int val)
        {
            var key = "Int Set Key{0}".AsFormat(val);
            Client.Set(key, val);

            try
            {
                var dbVal = Client.Get<int>(key);
                //if (dbVal == val)
                //    ctOfValInDb++;
            }
            catch (Exception e)
            {
                //valInDb = false;
            }
        }
    }
}
