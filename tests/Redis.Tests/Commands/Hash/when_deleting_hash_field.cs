using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Redis.Tests.Commands.Hash
{
    public class when_deleting_hash_field :
        with_clean_db
    {
        protected static bool DelRsltInitial;
        protected static bool DelRsltSecondTry;
        protected static bool ValExistsBeforeDel;
        protected static bool ValExistsAfterDel;

        private Because of = () =>
        {
            var key = "Test Hash Key Del";
            var field = "Test Hash Field Del";
            client.HSet(key, field, 1);
            ValExistsBeforeDel = client.HExists(key, field);
            DelRsltInitial = client.HDel(key, field);
            ValExistsAfterDel = client.HExists(key, field);
            DelRsltSecondTry = client.HDel(key, field);
        };

        private It should_get_a_success_result_on_deleting_a_valid_key_field_combination = () => DelRsltInitial.ShouldBeTrue();
        private It should_get_a_fail_result_on_deleting_an_invalid_key_field_combination = () => DelRsltSecondTry.ShouldBeFalse();
        private It should_find_a_valid_key_field_combination_after_setting = () => ValExistsBeforeDel.ShouldBeTrue();
        private It should_not_find_a_valid_key_field_combination_after_deleting = () => ValExistsAfterDel.ShouldBeFalse();

    }
}
