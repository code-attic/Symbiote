using System;
using Symbiote.Messaging;

namespace Messaging.Tests.RequestResponse
{
    public class BidProcessor
        : IHandle<Auction, MakeBid>
    {
        public Action<IEnvelope> Handle( Auction actor, MakeBid message )
        {
            var bid = new Bid(
                message.User,
                message.Item,
                message.Amount,
                message.MadeOn );
            var accepted = actor.AcceptBid(bid);
            return x => x.Acknowledge();
        }
    }
}