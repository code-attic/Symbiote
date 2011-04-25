using System;
using System.Runtime.Serialization;

namespace Rabbit.Tests
{
    [Serializable]
    [DataContract]
    public class Message
    {
        [DataMember(Order = 1)]
        public int Id { get;set; }
        [DataMember(Order = 2)]
        public string CorrelationId { get; set; }
    }
}