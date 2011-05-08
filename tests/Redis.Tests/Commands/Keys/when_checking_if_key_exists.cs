using Machine.Specifications;

namespace Redis.Tests.Commands.Keys
{
    public class when_checking_if_key_exists :
        with_clean_db
    {
        protected static bool realKeyExists;
        protected static bool fakeKeyDoesntExist;

        private Because of = () =>
        {
            var key = "Exists Key";
            client.Set(key, 1);
            realKeyExists = client.Exists(key);
            fakeKeyDoesntExist = client.Exists("Fake Key");
        };

        private It should_find_a_real_key = () => realKeyExists.ShouldBeTrue();
        private It should_not_find_a_fake_key = () => fakeKeyDoesntExist.ShouldBeFalse();

    }
}
