using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Messaging.Tests.RequestResponse
{
    public class AuctionKeyAccessor
        : IKeyAccessor<Auction>
    {
        public string GetId( Auction actor )
        {
            return actor.Item;
        }

        public void SetId<TKey>( Auction actor, TKey key )
        {
            actor.Item = key.ToString();
        }
    }
}