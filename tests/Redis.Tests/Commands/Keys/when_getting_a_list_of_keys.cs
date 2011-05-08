using Machine.Specifications;

namespace Redis.Tests.Commands.Keys
{
    public class when_getting_a_list_of_keys :
        with_clean_db
    {
        protected static string[] allKeyEnum;
        protected static string[] twoKeyEnum;
        protected static string[] singleKeyEnum;

        private Because of = () =>
        {
            var key1 = "Key List 1";
            var key2 = "Key List 2";
            client.Set(key1, 1);
            client.Set(key2, 2);

            allKeyEnum = client.Keys;
            twoKeyEnum = client.GetKeys("Key List*");
            singleKeyEnum = client.GetKeys("*2");

        };

        private It should_get_all_keys_when_attempted = () => allKeyEnum.Length.ShouldEqual(2);
        private It should_get_multiple_keys_for_pattern_match = () => twoKeyEnum.Length.ShouldEqual(2);
        private It should_get_limited_key_set_for_pattern_match = () => singleKeyEnum.Length.ShouldEqual(1);
    }
}