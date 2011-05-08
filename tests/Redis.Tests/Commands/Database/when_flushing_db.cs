using System;
using Machine.Specifications;

namespace Redis.Tests.Commands.Database
{
    public class when_flushing_all : with_redis_client
    {
        protected static string key;
        protected static int dbVal;
        protected static int ctOfValInDb = 0;
        protected static bool valInDb;

        private Because of = () =>
        {
            client.SelectDb(1);
            key = "Flush Key";
            client.Set(key, 12);
            try
            {
                dbVal = client.Get<int>(key);
                valInDb = true;
            }
            catch (Exception e)
            {
                valInDb = false;
                throw;
            }

            client.FlushDb();

            try
            {
                dbVal = client.Get<int>(key);
                valInDb = true;
            }
            catch (Exception e)
            {
                valInDb = false;
            }



        };

        private It should_value_flushed_from_db = () =>
        {
            valInDb.ShouldBeFalse();
        };

    }
}
