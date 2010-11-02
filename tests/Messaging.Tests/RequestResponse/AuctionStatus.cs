using System;
using System.Runtime.Serialization;

namespace Messaging.Tests.RequestResponse
{
    [Serializable]
    [DataContract]
    public class AuctionStatus
    {
        public string Item { get; set; }
        public string HighestBidder { get; set; }
        public bool BiddingClosed { get; set; }
        public decimal CurrentBid { get; set; }

        public AuctionStatus() {}

        public AuctionStatus( string item, string highestBidder, bool biddingClosed, decimal currentBid )
        {
            Item = item;
            HighestBidder = highestBidder;
            BiddingClosed = biddingClosed;
            CurrentBid = currentBid;
        }
    }
}