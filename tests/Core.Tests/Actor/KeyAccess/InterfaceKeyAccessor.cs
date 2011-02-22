using Symbiote.Core;

namespace Core.Tests.Actor.KeyAccess
{
    public class InterfaceKeyAccessor : IKeyAccessor<IHaveId>
    {
        public string GetId( IHaveId actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( IHaveId actor, TKey key )
        {
            actor.SetId( key.ToString() );
        }
    }
}