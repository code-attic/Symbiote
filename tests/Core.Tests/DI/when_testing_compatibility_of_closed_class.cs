using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI
{
    public class when_testing_compatibility_of_closed_class
    {
        public static bool compatible { get; set; }
        public static bool genericMatch { get; set; }

        private Because of = () =>
                                 {
                                     compatible = typeof(ClosedClass).IsConcreteAndAssignableTo( typeof(AnInterfaceOf));
                                     genericMatch = typeof(ClosedClass).GetGenericCardinality() == typeof(AnInterfaceOf).GetGenericCardinality();
                                 };

        private It should_be_compatible = () => compatible.ShouldBeTrue();
        private It should_have_cardinality_match = () => genericMatch.ShouldBeTrue();
    }
}