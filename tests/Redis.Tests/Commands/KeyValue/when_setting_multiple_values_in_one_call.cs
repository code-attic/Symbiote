using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Redis.Tests.Commands.KeyValue
{
    class when_setting_multiple_values_in_one_call : with_clean_db
    {
        protected static bool setRslt;
        protected static int dbVal;
        protected static int ctOfValInDb = 0;
        protected static long runMilliseconds;

        private Because of = () =>
        {

            var list = new Dictionary<string, int>();
            for (int i = 0; i < 1000; i++)
            {
                list.Add("Int Set Key{0}".AsFormat(i), i);
            }

            setRslt = client.Set(list);

            ctOfValInDb = client.DbSize;

            client.FlushDb();
        };

        private It should_return_success_on_call = () => setRslt.ShouldBeTrue();
        private It should_all_exist_in_the_db = () => ctOfValInDb.ShouldEqual(1000);


    }
}
