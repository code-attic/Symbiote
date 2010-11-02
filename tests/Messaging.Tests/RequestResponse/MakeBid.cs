using System;
using System.Runtime.Serialization;

namespace Messaging.Tests.RequestResponse
{
    [Serializable]
    [DataContract]
    public class MakeBid
    {
        [DataMember(IsRequired = false, Order = 1)] 
        public string User { get; set; }

        [DataMember(IsRequired = false, Order = 2)]
        public string Item { get; set; }

        [DataMember(IsRequired = false, Order = 3)]
        public decimal Amount { get; set; }

        [DataMember(IsRequired = false, Order = 4)]
        public DateTime MadeOn { get; set; }

        public MakeBid()
        {
            MadeOn = DateTime.Now;
        }

        public MakeBid(string user, string item, decimal amount)
            : this()
        {
            User = user;
            Item = item;
            Amount = amount;
        }
    }
}