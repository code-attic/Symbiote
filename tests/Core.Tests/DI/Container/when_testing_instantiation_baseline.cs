using System.Diagnostics;
using System.Linq;
using Machine.Specifications;
using IEnumerableExtenders = Symbiote.Core.Extensions.IEnumerableExtenders;

namespace Core.Tests.DI.Container
{
    public class when_testing_instantiation_baseline
    {
        protected static Stopwatch watch;

        private Because of = () =>
            {
                watch = Stopwatch.StartNew();

                IEnumerableExtenders.ForEach<int>( Enumerable
                                       .Range( 0, 5000 ), x => new MessageProvider( new MessageHazzer() ) );

                watch.Stop();
            };

        private It should_run_in_under_10_ms = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1 );
    }
}