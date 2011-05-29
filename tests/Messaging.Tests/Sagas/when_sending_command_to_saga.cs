using System;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Actor.Tests.Sagas
{
    public class when_sending_command_to_saga
        : with_bus
    {
        public static IAgent<Person> Agent { get; set; }
        public static Person person { get; set; }
        private Because of = () =>
                                 {
                                     Bus.Publish( "local", new SetPersonName() {Name = "Bob"} );
                                     Bus.Publish( "local", new SetPersonName() {Name = "Bob"} );
                                     Thread.Sleep( 50 );
                                     Agent = Assimilate.GetInstanceOf<IAgent<Person>>();
                                     person = Agent.GetActor( Guid.Empty.ToString() );
                                 };

        private It should_name_should_reset = () => person.Name.ShouldEqual( "Reset" );
    }
}