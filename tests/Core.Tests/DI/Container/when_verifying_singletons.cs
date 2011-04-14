using System.Linq;
using Machine.Specifications;
using IEnumerableExtenders = Symbiote.Core.Extensions.IEnumerableExtenders;

namespace Core.Tests.DI.Container
{
    public class when_verifying_singletons : with_singleton_registration
    {
        private static IShouldBeSingleton a;
        private static IShouldBeSingleton b;

        private Because of = () =>
            {
                IEnumerableExtenders.ForEach<int>( Enumerable
                                       .Range( 0, 50 )
                                       .AsParallel(), x => a = Container.GetInstance<IShouldBeSingleton>( "a" ) );
                
                IEnumerableExtenders.ForEach<int>( Enumerable
                                       .Range( 0, 50 )
                                       .AsParallel(), x => b = Container.GetInstance<IShouldBeSingleton>( "b" ) );
            };

        private It should_only_have_single_a_instance = () => a.Instance.ShouldEqual( 0 );
        private It should_only_have_single_b_instance = () => b.Instance.ShouldEqual( 0 );
    }
}