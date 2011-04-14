using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public class when_instantiating_closed_generic : with_generics_registration
    {
        private static bool passed;

        private Because of = () =>
            {
                var instance = Container.GetInstance( typeof(ITakeGenericParams<>), "closed" );
                passed = instance.GetType().Equals( typeof( ClosedImpl ) );
            };

        private It should_match_passed_type = () => passed.ShouldBeTrue();
    }
}