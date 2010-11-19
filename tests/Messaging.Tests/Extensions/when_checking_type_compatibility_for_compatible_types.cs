using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Messaging.Extensions;

namespace Messaging.Tests.Extensions
{
    public class when_checking_type_compatibility_for_compatible_types
    {
        protected static bool IsBaseMatching { get; set; }
        protected static bool IsInterfaceMatching { get; set; }

        private Because of = () =>
        {
            var msg = new ChildType();
            IsBaseMatching = msg.IsOfType<BaseType>();
            IsInterfaceMatching = msg.IsOfType<InterfaceType>();
        };

        private It should_report_interface_match = () => IsBaseMatching.ShouldBeTrue();
        private It should_report_base_match = () => IsBaseMatching.ShouldBeTrue();
    }
}
