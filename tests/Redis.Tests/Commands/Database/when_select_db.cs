using System;
using Machine.Specifications;

namespace Redis.Tests.Commands.Database
{
    public class when_select_db: with_redis_client
    {
        protected static bool valInDb = false;
        private Because of = () =>
        {
            client.SelectDb(1);
            client.FlushDb();
            client.Set("Db Check", 1);
            client.SelectDb(2);
            client.FlushDb();
            client.Set("Db Check", 2);
            client.SelectDb(1);

            try
            {
                var dbVal = client.Get<int>("Db Check");
                valInDb = dbVal == 1;
            }
            catch (Exception e)
            {
                valInDb = false;
            }

            client.SelectDb(1);
            client.FlushDb();
            client.SelectDb(2);
            client.FlushDb();
            client.SelectDb(0);


        };

        private It should_all_exist_in_the_db = () =>
        {
            valInDb.ShouldBeTrue();
        };

    }
}
