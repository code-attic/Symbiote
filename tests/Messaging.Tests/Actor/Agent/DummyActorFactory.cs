using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Agent
{
    public class DummyActorFactory
        : IActorFactory<DummyActor>
    {
        public int Called { get; set; }

        public DummyActor CreateInstance<TKey>( TKey id )
        {
            Called++;
            return new DummyActor();
        }
    }
}