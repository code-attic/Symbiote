using System;
using System.Collections.Generic;

namespace Messaging.Tests.RequestResponse
{
    public class Auction
    {
        public string Item { get; set; }
        public Bid CurrentBid { get; set; }
        public List<Bid> BidHistory { get; set; }
        public DateTime Expires { get; set; }

        public bool AcceptBid(Bid bid)
        {
            var accepted = false;
            if(Expires < DateTime.Now)
            {
                BidHistory.Add( bid );
                CurrentBid = bid;
                accepted = true;
            }
            return accepted;
        }

        public Auction()
        {
            BidHistory = new List<Bid>();
        }
    }
}