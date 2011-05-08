using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Hash
{
    public class when_checking_if_hash_key_value_exists :
        with_clean_db
    {
        protected static bool ValExistsBeforeSet;
        protected static bool ValExistsAfterSet;

        private Because of = () =>
        {
            var key = "Test Hash Key Exists";
            var field = "Test Hash Field Exists";
            ValExistsBeforeSet = client.HExists(key, field);
            client.HSet(key, field, 1);
            ValExistsAfterSet = client.HExists(key, field);
        };

        private It should_not_find_a_valid_key_field_combination_before_setting = () => ValExistsBeforeSet.ShouldBeFalse();
        private It should_find_a_valid_key_field_combination_after_setting = () => ValExistsAfterSet.ShouldBeTrue();

    }
}
