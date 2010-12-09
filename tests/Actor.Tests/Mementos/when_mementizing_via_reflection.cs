using Actor.Tests.Domain.Model;
using Machine.Specifications;
using Symbiote.Actor.Impl.Memento;
using Symbiote.Core;

namespace Actor.Tests.Mementos
{
    public class when_mementizing_via_reflection
        : with_driver
    {
        protected static Driver clone { get; set; }
        protected static Driver reset { get; set; }

        private Because of = () =>
        {
            var memoizer = Assimilate.GetInstanceOf<IMemoizer>();
            var memento = memoizer.GetMemento( driver );
            clone = memento.Retrieve();
            reset = memento.Retrieve();
            reset.ChangeName( "Kaptain", "Kangaroo" );
            memento.Reset( reset );
        };

        private It should_produce_different_instance_from_original = () => ReferenceEquals( driver, clone ).ShouldBeFalse();
        private It should_reset_correctly = () => (reset.FirstName == "Mr" && reset.LastName == "Rogers").ShouldBeTrue();
    }
}