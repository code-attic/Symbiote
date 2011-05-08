using Machine.Specifications;

namespace Redis.Tests.Commands.Keys
{
    public class when_checking_ttl :
        with_clean_db
    {
        protected static int ttlNotExists;
        protected static int ttlNoExpire;
        protected static int ttlWithExpire;

        private Because of = () =>
        {
            var key = "TTL Key";
            ttlNotExists = client.TimeToLive(key);
            client.Set(key, 1);
            ttlNoExpire = client.TimeToLive(key);
            client.Expire(key, 1);
            ttlWithExpire = client.TimeToLive(key);
        };

        private It should_get_negative_one_when_key_does_not_exist = () => ttlNotExists.ShouldEqual(-1);
        private It should_get_negative_one_when_key_does_not_expire = () => ttlNoExpire.ShouldEqual(-1);
        private It should_get_a_positive_value_when_key_has_expire = () => ttlWithExpire.ShouldBeGreaterThan(0);

    }
}
