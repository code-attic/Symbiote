using Symbiote.Actor;

namespace Actor.Tests.Agent
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