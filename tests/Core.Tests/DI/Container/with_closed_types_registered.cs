using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI.Container
{
    public class with_closed_types_registered : with_container
    {
        private Establish context = () =>
            {
                var def1 = DependencyExpression.For<AClassOf<string>>();
                def1.Use<ClosedClass>();
                Container.Register( def1 );
            };
    }
}