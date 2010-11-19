using Machine.Specifications;
using Symbiote.Messaging.Extensions;

namespace Messaging.Tests.Extensions
{
    public class when_checking_type_compatibility_for_incompatible_types
    {
        protected static bool IsBaseMatching { get; set; }
        protected static bool IsInterfaceMatching { get; set; }

        private Because of = () =>
        {
            var msg = new IndependentType();
            IsBaseMatching = msg.IsOfType<BaseType>();
            IsInterfaceMatching = msg.IsOfType<InterfaceType>();
        };

        private It should_report_interface_mismatch = () => IsInterfaceMatching.ShouldBeFalse();
        private It should_report_base_mismatch = () => IsBaseMatching.ShouldBeFalse();
    }
}