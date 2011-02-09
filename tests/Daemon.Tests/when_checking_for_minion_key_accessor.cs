using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Daemon.BootStrap;

namespace Daemon.Tests
{
    public class when_checking_for_minion_key_accessor
        : with_assimilation
    {
        public static IKeyAccessor<Minion> MinionKeyAccessor { get; set; }

        private Because of = () =>
                                 {
                                     MinionKeyAccessor = Assimilate.GetInstanceOf<IKeyAccessor<Minion>>();
                                 };

        private It should_have_accessor = () => MinionKeyAccessor.ShouldNotBeNull();
    }
}