using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Core.Tests.DecisionTree
{
    public class when_running_thingy_saga : with_assimilation
    {
        static IBus Bus { get; set; }

        private Establish context = () => 
                                        { 
                                            Bus = Assimilate.GetInstanceOf<IBus>(  );
                                            Bus.AddLocalChannel( x => x.CorrelateBy<ProcessThingy>( p => p.ThingyId ) );

                                        };

        private Because of = () => 
                                 { 
                                     Bus.Publish( "local", new ProcessThingy("test") );
                                     Thread.Sleep( 100 );
                                 };

        private It should_have_processed_hit_1 = () => ThingySaga.Hit1.ShouldBeTrue();
        private It should_have_processed_hit_2 = () => ThingySaga.Hit2.ShouldBeTrue();
        private It should_have_processed_hit_3 = () => ThingySaga.Hit3.ShouldBeTrue();
    }
}