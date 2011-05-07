using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.KeyValue
{
    public class when_check_setting_value :
        with_clean_db
    {
        protected static int initialVal;
        protected static int replacementVal;
        protected static string key;
        protected static int dbVal1;
        protected static int dbVal2;
        protected static bool valSetSuccessful1;
        protected static bool valSetSuccessful2;

        private Because of = () =>
        {
            initialVal = 0;
            replacementVal = 1;
            key = "Int CheckSet Key";
            valSetSuccessful1 = client.CheckAndSet(key, initialVal);
            valSetSuccessful2 = client.CheckAndSet(key, replacementVal);
            dbVal1 = client.Get<int>(key);
            dbVal2 = client.Get<int>(key);
        };

        private It should_set_the_value_on_the_first_call = () => valSetSuccessful1.ShouldBeTrue();
        private It should_not_set_the_value_on_the_second_call = () => valSetSuccessful2.ShouldBeFalse();
        private It first_retrieved_val_should_equal_the_original_value = () => dbVal1.ShouldEqual(initialVal);
        private It second_retrieved_val_should_equal_the_original_value = () => dbVal2.ShouldEqual(initialVal);
    }
}