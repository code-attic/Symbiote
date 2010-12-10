using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auction.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }

    public class Bid
        : Entity
    {
        public Guid BidderId { get; protected set; }
        public decimal Amount { get; protected set; }
        public DateTime PlacedOn { get; protected set; }

        public Bid( Guid bidderId, decimal amount, DateTime placedOn )
        {
            BidderId = bidderId;
            Amount = amount;
            PlacedOn = placedOn;
        }
    }

    public class Auction
        : Entity
    {
        public Guid OwnerId { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public decimal Increment { get; protected set; }

        public Bid CurrentBid { get; protected set; }
        public IList<Bid> BidHistory { get; protected set; }
    }


}
