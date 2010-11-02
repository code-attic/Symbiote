using System;
using System.Runtime.Serialization;

namespace Messaging.Tests.MessageSerializers
{

    [Serializable]
    [DataContract]
    public class CompatibleMessage
    {
        [DataMember(IsRequired=false,Order=1)]
        public string Text { get; set; }
        public CompatibleMessage() {}
    }
}
