using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Core.Tests.Actor.Agent
{
    public class when_instantiating_from_actor
        : with_assimilation
    {
        public static DefaultAgent<DummyActor> Agent { get; set; }
        public static DummyActorFactory Factory { get; set; }

        private Because of = () =>
                                 {
                                     Agent = Assimilate.GetInstanceOf<IAgent<DummyActor>>() as DefaultAgent<DummyActor>;
                                     var testAgent = Assimilate.GetInstanceOf<IAgent<DummyActor>>();
                                     
                                     Factory = Assimilate.GetInstanceOf<IActorFactory<DummyActor>>() as DummyActorFactory;
                                     var instance1 = testAgent.GetActor( "test" );
                                     var instance2 = testAgent.GetActor( "test" );
                                 };

        Cleanup reset_counter = () =>
                                    {
                                        Factory.Called = 0;
                                        DummyActor.Instantiated = 0;
                                    };

        private It should_have_cached_instance_in_memory = () => Agent.Actors.Count.ShouldEqual( 1 );
    }
}