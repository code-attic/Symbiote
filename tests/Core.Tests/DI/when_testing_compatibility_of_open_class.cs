using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI
{
    public class when_testing_compatibility_of_open_class
    {
        public static bool compatible { get; set; }
        public static bool genericMatch { get; set; }

        private Because of = () =>
                                 {
                                     compatible = typeof(AClassOf<string>).IsConcreteAndAssignableTo( typeof(AnInterfaceOf) );
                                     genericMatch = typeof(AClassOf<>).GetGenericCardinality() == typeof(AnInterfaceOf).GetGenericCardinality();
                                 };

        private It should_be_compatible = () => compatible.ShouldBeTrue();
        private It should_not_have_cardinality_match = () => genericMatch.ShouldBeFalse();
    }
}