using Symbiote.Messaging;

namespace Messaging.Tests.RequestResponse
{
    public class BidProcessor
        : IHandle<Auction, MakeBid>
    {
        public void Handle( Auction actor, IEnvelope<MakeBid> envelope )
        {
            var bid = new Bid(
                envelope.Message.User,
                envelope.Message.Item,
                envelope.Message.Amount,
                envelope.Message.MadeOn );
            var accepted = actor.AcceptBid(bid);


        }


    }
}