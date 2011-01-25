using System;
using System.Runtime.Serialization;

namespace Messages
{
    [DataContract, Serializable]
    public class OpenNewAccount
    {
        [DataMember(Order = 1)]
        public string UserId { get; set; }
        [DataMember(Order = 2)]
        public decimal Amount { get; set; }

        public OpenNewAccount() { }

        public OpenNewAccount(string userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
        }
    }
}