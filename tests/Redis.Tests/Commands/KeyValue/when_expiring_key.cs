using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    class when_expiring_key :
        with_clean_db
    {
        protected static string key;
        protected static bool expireSet;
        protected static bool valExistsAfterExpire;

        private Because of = () =>
        {
            key = "Expire Key";
            client.Set(key, 1);
            expireSet = client.Expire(key, 1);
            Thread.Sleep( 1500 );
      //      valExistsAfterExpire = 





        };

        //private It should_step_down_by_one = () => dbVal2.ShouldEqual(dbVal1 - 1);
        //private It should_step_down_by_one_repeatedly = () => dbVal3.ShouldEqual(dbVal2 - 4);
//        private It should_step_down_multiple_in_one_call = () => dbVal2.ShouldEqual(dbVal3 - 5);

    }
}
