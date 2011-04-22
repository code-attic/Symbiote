using System.Diagnostics;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.DI.Container
{
    public class when_testing_simple_instantiation_scenario : with_simple_registration
    {
        protected static Stopwatch watch;
        protected static bool pass;
        private Because of = () =>
            {
                Container.GetInstance<IMessageProvider>();
                watch = Stopwatch.StartNew();

                pass = Enumerable
                    .Range( 0, 5000 )
                    .All( x => Container.GetInstance<IMessageProvider>().GetMessage() == "This is a message from MessageHazzer. Hi!" );

                watch.Stop();
            };

        private It should_run_in_under_10_ms = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 10 );
    }
}