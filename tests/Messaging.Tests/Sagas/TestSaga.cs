using System;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Saga;

namespace Actor.Tests.Sagas
{
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
                                                           return e => e.Acknowledge();
                                                       });

                           x.When( p => p.Initialized )
                               .On<SetPersonName>( ( p, m ) =>
                                                       {
                                                           p.Name = "Reset";
                                                           return e => e.Acknowledge();
                                                       } );
                       };
        }

        public TestSaga( StateMachine<Person> stateMachine ) : base( stateMachine ) {}
    }
}