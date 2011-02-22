using Symbiote.Core;

namespace Core.Tests.Json.Decorator
{
    public class HaveIdKeyAccessor
        : IKeyAccessor<IHaveKey>
    {
        public string GetId( IHaveKey actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( IHaveKey actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }
}