using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Actor.Tests.Agent
{
    public class when_creating_actor_instance_with_default_factory
        : with_assimilation
    {
        public static DummyActor Actor { get; set; }

        private Because of = () =>
                                 {
                                     var factory = Assimilate.GetInstanceOf<DefaultActorFactory<DummyActor>>();
                                     Actor = factory.CreateInstance( "test" );
                                 };

        private It should_get_valid_instance = () => Actor.ShouldNotBeNull();
        private It should_have_correct_id = () => Actor.Id.ShouldEqual( "test" );
    }
}