using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    public class when_incrementing :
        with_clean_db
    {
        protected static int dbVal1;
        protected static int dbVal2;
        protected static int dbVal3;
        protected static int dbVal4;

        private Because of = () =>
        {
            var initialVal = 0;
            var finalVal = 10;
            dbVal1 = initialVal;
            var key = "Int Increment Key";
            client.Set(key, initialVal);
            dbVal2 = client.Increment(key);
            for (int i = 0; i < 4; i++)
            {
                dbVal3 = client.Increment(key);
            }

            dbVal4 = client.Increment(key, 5);


        };

        private It should_step_up_by_one = () => dbVal2.ShouldEqual(dbVal1 + 1);
        private It should_step_up_by_one_repeatedly = () => dbVal3.ShouldEqual(dbVal2 + 4);
        private It should_step_up_multiple_in_one_call = () => dbVal4.ShouldEqual(dbVal3 + 5);

    }
}
