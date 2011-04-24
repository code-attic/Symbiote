using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI.Container
{
    public class with_duplicate_singletons_registered : with_container
    {
        private Establish context = () =>
            {
                var def1 = DependencyExpression.For<IShouldBeSingleton>();
                def1.Use( new SingletonA() { Message = "first" } );

                var def2 = DependencyExpression.For<IShouldBeSingleton>();
                def2.Use( new SingletonB() { Message = "second" } );

                Container.Register( def1 );
                Container.Register( def2 );
            };
    }
}