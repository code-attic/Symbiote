using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Database
{
    class when_decrementing :
        with_clean_db
    {
        protected static int initialVal;
        protected static int finalVal;
        protected static string key;
        protected static int dbVal1;
        protected static int dbVal2;
        protected static int dbVal3;
        protected static int dbVal4;

        private Because of = () =>
        {
            initialVal = 10;
            finalVal = 0;
            dbVal1 = initialVal;
            key = "Int Decrement Key";
            client.Set(key, initialVal);
            dbVal2 = client.Decrement(key);
            for( int i = 0; i < 4; i++ )
            {
                dbVal3 = client.Decrement(key);
            }

            dbVal4 = client.Decrement(key, 5);


        };

        private It should_step_down_by_one = () => dbVal2.ShouldEqual(dbVal1 -1);
        private It should_step_down_by_one_repeatedly = () => dbVal3.ShouldEqual(dbVal2 -4);
        private It should_step_down_multiple_in_one_call = () => dbVal2.ShouldEqual(dbVal3 -5);

    }
}
