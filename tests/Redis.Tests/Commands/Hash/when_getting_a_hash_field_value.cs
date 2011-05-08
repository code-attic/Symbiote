using Machine.Specifications;

namespace Redis.Tests.Commands.Hash
{
    public class when_getting_a_hash_field_value :
        with_clean_db
    {
        protected static bool GetRsltCorrect;

        private Because of = () =>
        {
            var key = "Test Hash Key Get";
            var field = "Test Hash Field Get";
            client.HSet(key, field, 1);
            GetRsltCorrect = client.HGet<int>(key, field) == 1;
        };

        private It should_get_a_success_result_on_first_setting_a_valid_key_field_combination = () => GetRsltCorrect.ShouldBeTrue();

    }
}
