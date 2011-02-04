using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool Intiialized { get; set; }
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
        public string Name { get; set; }
    }

    public class TestSaga
        : Saga<Person>
    {
        public override Action<StateMachine<Person>> Setup()
        {
            return x => x.When( p => !p.Intiialized ).On<SetPersonName>( ( p, m ) => p.Name = m.Name );
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
}
