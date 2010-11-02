using System;
using System.Runtime.Serialization;

namespace Messaging.Tests.MessageSerializers
{
    [DataContract]
    [Serializable]
    public class BinaryAndJsonMessage
    {
        [DataMember(Order = 1)]
        public string Text { get; set; }

        //This member is unmarked but public
        // breaking protobuf compatibility
        public int Number { get; set; }
    }
}