using Machine.Specifications;
using Symbiote.Core.DI;

namespace Core.Tests.DI.Container
{
    public class when_requesting_concrete_open_type : with_simple_registration
    {
        private static string message;
        private Because of = () =>
            {
                var instance = Container.GetInstance<OpenConcrete<string>>();
                message = instance.Provider.GetMessage();
            };

        private It should_have_correct_message = () => message.ShouldEqual( "This is a message from MessageHazzer. Hi!" );
    }

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

    public class when_testing_duplicate_singleton_definition : with_duplicate_singletons_registered
    {
        private static bool pass;
        private Because of = () =>
            {
                pass = ( Container.GetInstance<IShouldBeSingleton>() as SingletonB ).Message == "second";
            };

        private It should_overwrite_initial_dependency_with_latter = () => pass.ShouldBeTrue();
    }
}