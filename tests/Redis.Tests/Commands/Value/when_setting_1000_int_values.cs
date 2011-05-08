using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.KeyValue
{
    public class when_setting_1000_int_values: with_clean_db
    {
        protected static string key;
        protected static int dbVal;
        protected static int ctOfValInDb= 0;
        protected static long runMilliseconds;

        private Because of = () =>
        {

            Stopwatch sw = Stopwatch.StartNew();
            for( int i = 0; i < 1000; i++ )
            {
                key = "Int Set Key{0}".AsFormat(i);
                client.Set(key, i);

                try
                {
                    dbVal = client.Get<int>(key);
                    if (dbVal == i) 
                        ctOfValInDb++;
                }
                catch (Exception e)
                {
                    //valInDb = false;
                }
            }

            sw.Stop();
            runMilliseconds = sw.ElapsedMilliseconds;
            client.FlushDb();
        };

        private It should_all_exist_in_the_db = () =>
        {
            ctOfValInDb.ShouldEqual(1000);
        };

        //private It should_run_in_less_than_half_a_second = () =>
        //{
        //    runMilliseconds.ShouldBeLessThan(500);
        //};

    }
}
