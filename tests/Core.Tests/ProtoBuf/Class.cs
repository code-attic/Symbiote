using System.Runtime.Serialization;

namespace Core.Tests.ProtoBuf
{
    [DataContract]
    public class Class : Base
    {
        [DataMember(Order = 20)]
        public string ClassProperty { get; set; }
    }
}