using System;
using System.Runtime.Serialization;

namespace RabbitDemo.Messages
{
    //[DataContract]
    public class Message
    {
        //[DataMember(Order = 1)]
        public string CorrelationId { get; set; }
        //[DataMember(Order = 2)]
        public int MessageId { get; set; }
        //[DataMember(Order = 3)]
        public DateTime TimeStamp { get; set; }

        public Message() {}

        public Message(string correlationId, int id)
        {
            CorrelationId = correlationId;
            MessageId = id;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
