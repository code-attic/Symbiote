using System;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Reflection;

namespace Core.Tests.Reflection.Inheritence
{
    public class when_retrieving_inheritance_chain
    {
        public static Type[] Chain { get; set; }

        private Because of = () =>
        {
            Chain = Enumerable.ToArray<Type>( Reflector.GetInheritanceChain( typeof(MyClass) ) );
        };

        private It should_have_3_results = () => 
                        Chain.Length.ShouldEqual( 3 );
        private It should_have_interface_a = () => 
                        Chain.Any(x => x.Equals(typeof(InterfaceA))).ShouldBeTrue();
    }
}