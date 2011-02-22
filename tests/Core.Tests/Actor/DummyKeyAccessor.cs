using Symbiote.Core;

namespace Core.Tests.Actor
{
    public class DummyKeyAccessor : IKeyAccessor<DummyActor>
    {
        public string GetId( DummyActor actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( DummyActor actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }
}