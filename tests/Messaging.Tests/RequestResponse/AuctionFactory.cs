using Symbiote.Actor;

namespace Messaging.Tests.RequestResponse
{
    public class AuctionFactory
        : IActorFactory<Auction>
    {
        public Auction CreateInstance<TKey>( TKey id )
        {
            return new Auction()
            {
                Item = id.ToString()
            };
        }
    }
}