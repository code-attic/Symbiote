using System.Diagnostics;
using System.Linq;
using Machine.Specifications;
using StructureMap;

namespace Core.Tests.DI.Container
{
    public class when_testing_structuremap_instantiation_scenario : with_simple_structuremap_registration
    {
        protected static Stopwatch watch;
        protected static bool pass;
        private Because of = () =>
            {
                ObjectFactory.GetInstance<IMessageProvider>();
                watch = Stopwatch.StartNew();

                pass = Enumerable
                    .Range( 0, 5000 )
                    .All( x => ObjectFactory.GetInstance<IMessageProvider>().GetMessage() == "This is a message from MessageHazzer. Hi!" );

                watch.Stop();
            };

        private It should_run_in_under_10_ms = () => watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 10 );
    }
}