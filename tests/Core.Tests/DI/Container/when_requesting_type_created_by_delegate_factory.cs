using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI.Container
{
    public class with_registered_delegate : with_container
    {
        private Establish context = () =>
                                        {
                                            var def1 = DependencyExpression.For<string>();
                                            def1.CreateWith( x => "Hello World!" );

                                            Container.Register( def1 );
                                        };
    }

    public class when_requesting_type_created_by_delegate_factory : with_registered_delegate
    {
        private static string result;

        private Because of = () =>
                                 {
                                     result = Container.GetInstance<string>();
                                 };

        private It should_show_as_registered = () => Container.HasPluginFor<string>().ShouldBeTrue();

        private It should_return_expected_string = () => result.ShouldEqual( "Hello World!" );
    }
}
