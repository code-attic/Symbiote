using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    public class when_getting_and_setting_int_value :
        with_clean_db
    {
        protected static int initialVal;
        protected static int replacementVal;
        protected static int dbVal1;
        protected static int dbVal2;
        protected static bool valInDb;

        private Because of = () =>
        {
            initialVal = 0;
            replacementVal = 1;
            var key = "Int GetSet Key";
            client.Set(key, initialVal);
            dbVal1 = client.GetSet(key, replacementVal);
            dbVal2 = client.Get<int>(key  );
            valInDb = true;
        };

        private It should_exist_in_the_db = () =>
            valInDb.ShouldBeTrue();
        private It first_retrieved_val_should_equal_the_original_value = () => dbVal1.ShouldEqual(initialVal);
        private It second_retrieved_val_should_equal_the_replaced_value = () => dbVal2.ShouldEqual(replacementVal);

    }
}
