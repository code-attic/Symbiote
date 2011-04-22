using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public class when_instantiating_open_generic : with_generics_registration
    {
        private static bool passed;

        private Because of = () =>
            {
                var instance = Container.GetInstance<ITakeGenericParams<int>>();
                passed = instance.GetTypeOfT.Equals( typeof( int ) );
            };

        private It should_match_passed_type = () => passed.ShouldBeTrue();
    }
}