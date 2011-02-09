using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Messaging.Tests;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Saga;

namespace Actor.Tests.Sagas
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Initialized { get; set; }

        public Person()
        {
            Id = Guid.Empty;
        }
    }

    public class PersonKeyAccessor : IKeyAccessor<Person>
    {
        public string GetId( Person actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Person actor, TKey id )
        {
            actor.Id = Guid.Parse( id.ToString() );
        }
    }

    public class PersonFactory : IActorFactory<Person>
    {
        public Person CreateInstance<TKey>( TKey id )
        {
            return new Person();
        }
    }

    public class SetPersonName
    {
        public Guid PersonId { get; set; }
        public string Name { get; set; }

        public SetPersonName()
        {
            PersonId = Guid.Empty;
        }
    }

    public class TestSaga
        : Saga<Person>
    {
        public override Action<StateMachine<Person>> Setup()
        {
            return x =>
                       {
                           x.When( p => !p.Initialized )
                               .On<SetPersonName>( ( p, m ) =>
                                                       {
                                                           p.Name = m.Name;
                                                           p.Initialized = true;
                                                       });

                           x.When( p => p.Initialized )
                               .On<SetPersonName>( ( p, m ) => 
                                   p.Name = "Reset" );
                       };
        }

        public TestSaga( StateMachine<Person> stateMachine ) : base( stateMachine ) {}
    }

    public class when_requesting_saga
        : with_assimilation
    {
        public static IEnumerable<ISaga> Sagas { get; set; }

        private Because of = () =>
        {
            Sagas = Assimilate.GetAllInstancesOf<ISaga>();
        };

        private It should_not_be_empty = () => 
            Sagas.Count().ShouldBeGreaterThan( 0 );
    }

    public class with_bus
        : with_assimilation
    {
        public static IBus Bus { get; set; }
        private Establish context = () =>
                                        {
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                            Bus.AddLocalChannel(x => x.CorrelateBy<SetPersonName>( m => m.PersonId.ToString() ));
                                        };
    }

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
