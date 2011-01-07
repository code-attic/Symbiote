using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace Core.Tests.DI
{
    public interface IAmAnInterface
    {
        void ThatDoesStuff();
    }

    public abstract class BaseClass : IAmAnInterface
    {
        public void ThatDoesStuff()
        {
            
        }
    }

    public class InheritsBaseClass : BaseClass
    {
        
    }

    public abstract class with_assembly_scanning
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Dependencies(x => x.Scan(s =>
                                                                              {
                                                                                  s.AssemblyContainingType<IAmAnInterface>();
                                                                                  s.AddAllTypesOf<IAmAnInterface>();
                                                                              }));
                                        };

    }

    public class when_loading_auto_wired_types
        : with_assembly_scanning
    {
        protected static IEnumerable<IAmAnInterface> instances { get; set; }

        private Because of = () =>
                                 {
                                     instances = Assimilate.GetAllInstancesOf<IAmAnInterface>();
                                 };

        private It should_only_provide_concrete_instance = () => instances.Count().ShouldEqual(1);
        private It should_have_type_of_concrete = () => instances.First().ShouldBeOfType<InheritsBaseClass>();
    }
}
