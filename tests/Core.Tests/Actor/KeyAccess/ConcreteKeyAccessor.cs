using Symbiote.Core;

namespace Core.Tests.Actor.KeyAccess
{
    public class ConcreteKeyAccessor : IKeyAccessor<ConcreteType>
    {
        public string GetId( ConcreteType actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( ConcreteType actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }
}