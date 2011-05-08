using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Hash
{
    public class when_setting_a_hash_field_value :
        with_clean_db
    {
        protected static bool SetRsltInitial;
        protected static bool SetRsltSecondTry;
        protected static bool ValExistsBeforeSet;
        protected static bool ValExistsAfterSet;
        protected static bool ValSameAfterSecondSet;
        protected static bool ValCorrectAfterFirstSet;

        private Because of = () =>
        {
            var key = "Test Hash Key Set";
            var field = "Test Hash Field Set";
            ValExistsBeforeSet = client.HExists(key, field);
            SetRsltInitial= client.HSet(key, field, 1);
            ValExistsAfterSet = client.HExists(key, field);
            ValCorrectAfterFirstSet = client.HGet<int>(key, field) == 1;
            SetRsltSecondTry = client.HSet(key, field, 2);
            ValSameAfterSecondSet = client.HGet<int>(key, field) == 1;

        };

        private It should_get_a_success_result_on_first_setting_a_valid_key_field_combination = () => SetRsltInitial.ShouldBeTrue();
        private It should_get_the_same_value_from_db_after_setting_a_valid_key_field_combination = () => ValCorrectAfterFirstSet.ShouldBeTrue();
        private It should_get_the_initial_value_from_db_after_setting_an_existing_key_field_combination = () => ValCorrectAfterFirstSet.ShouldBeTrue();
        private It should_get_a_fail_result_on_setting_key_field_combination_that_already_exists = () => SetRsltSecondTry.ShouldBeFalse();
        private It should_find_a_valid_key_field_combination_after_setting = () => ValExistsAfterSet.ShouldBeTrue();
        private It should_not_find_a_valid_key_field_combination_before_setting = () => ValExistsBeforeSet.ShouldBeFalse();

    }
}
