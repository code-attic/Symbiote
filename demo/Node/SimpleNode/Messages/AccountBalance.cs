using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Messages
{
    [DataContract, Serializable]
    public class AccountBalance
    {
        [DataMember(Order = 1)]
        public string UserId { get; set; }
        [DataMember(Order = 2)]
        public decimal Amount { get; set; }

        public AccountBalance() {}

        public AccountBalance( string userId, decimal amount )
        {
            UserId = userId;
            Amount = amount;
        }
    }
}
