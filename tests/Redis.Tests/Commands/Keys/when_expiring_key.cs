using System.Threading;
using Machine.Specifications;

namespace Redis.Tests.Commands.Keys
{
    public class when_expiring_key :
        with_clean_db
    {
        protected static bool expireSet;
        protected static bool valExistsAfterExpire;
        protected static bool valExistsBeforeExpire;

        private Because of = () =>
        {
            var key = "Expire Key";
            client.Set(key, 1);
            expireSet = client.Expire(key, 1);
            Thread.Sleep( 500 );
            valExistsBeforeExpire = client.Exists(key);
            Thread.Sleep( 1500 );
            valExistsAfterExpire = client.Exists(key);
        };

        private It should_accept_an_expire_time = () => expireSet.ShouldBeTrue();
        private It should_exist_before_expire_time = () => valExistsBeforeExpire.ShouldBeTrue();
        private It should_not_exist_after_expire_time = () => valExistsAfterExpire.ShouldBeFalse();

    }
}
