using Symbiote.Core;

namespace Rabbit.Tests
{
    public class ActorKeyAccessor : IKeyAccessor<Actor>
    {
        public string GetId( Actor actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( Actor actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }
}