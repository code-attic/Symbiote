using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.DI
{
    public class when_retrieving_scanned_types
        : with_assembly_scanning_for_marker_interface
    {
        protected static IEnumerable<AnInterfaceOf> instances { get; set; }

        private Because of = () =>
                                 {
                                     instances = Assimilate.GetAllInstancesOf<AnInterfaceOf>();
                                 };

        private It should_have_one_concrete_instance = () => 
                                                       ShouldExtensionMethods.ShouldEqual( instances.Count(), 1 );
    }
}