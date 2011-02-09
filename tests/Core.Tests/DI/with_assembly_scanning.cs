using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.StructureMapAdapter;

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

    public interface AnInterfaceOf
    {

    }

    public interface AnInterfaceOf<T> : AnInterfaceOf
    {
        
    }

    public class AClassOf<T> : AnInterfaceOf<T>
    {

    }

    public class ClosedClass : AClassOf<string>
    {
        
    }

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
                                                                                  s.AddSingleImplementations();
                                                                              }));
                                        };

    }

    public abstract class with_assembly_scanning_for_marker_interface
    {
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Dependencies(x => x.Scan(s =>
                {
                    s.AssemblyContainingType<IAmAnInterface>();
                    s.AddAllTypesOf<AnInterfaceOf>();
                }));
        };

    }

    public class when_retrieving_scanned_types
        : with_assembly_scanning_for_marker_interface
    {
        protected static IEnumerable<AnInterfaceOf> instances { get; set; }

        private Because of = () =>
        {
            instances = Assimilate.GetAllInstancesOf<AnInterfaceOf>();
        };

        private It should_have_one_concrete_instance = () => 
            instances.Count().ShouldEqual( 1 );
    }

    public class when_loading_auto_wired_types
        : with_assembly_scanning
    {
        protected static IEnumerable<IAmAnInterface> instances { get; set; }

        private Because of = () =>
                                 {
                                     instances = Assimilate.GetAllInstancesOf<IAmAnInterface>();
                                 };

        private It should_only_provide_concrete_instance = () => 
            instances.Count().ShouldEqual(1);
        private It should_have_type_of_concrete = () => instances.First().ShouldBeOfType<InheritsBaseClass>();
    }
}
