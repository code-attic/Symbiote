using System;

namespace Messaging.Tests.RequestResponse
{
    public class Bid
    {
        public string User { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public DateTime MadeOn { get; set; }

        public Bid( string user, string item, decimal amount, DateTime madeOn )
        {
            User = user;
            Item = item;
            Amount = amount;
            MadeOn = madeOn;
        }
    }
}