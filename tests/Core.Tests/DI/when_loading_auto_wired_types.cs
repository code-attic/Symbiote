using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.DI
{
    public class when_loading_auto_wired_types
        : with_assembly_scanning
    {
        protected static IEnumerable<IAmAnInterface> instances { get; set; }

        private Because of = () =>
                                 {
                                     instances = Assimilate.GetAllInstancesOf<IAmAnInterface>();
                                 };

        private It should_only_provide_concrete_instance = () => 
                                                           ShouldExtensionMethods.ShouldEqual( instances.Count(), 1);
        private It should_have_type_of_concrete = () => ShouldExtensionMethods.ShouldBeOfType<InheritsBaseClass>( instances.First() );
    }
}