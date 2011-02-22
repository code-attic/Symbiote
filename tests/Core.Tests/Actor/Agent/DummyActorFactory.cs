using Symbiote.Core.Actor;

namespace Core.Tests.Actor.Agent
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