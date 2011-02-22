using System.Runtime.Serialization;
using ProtoBuf;

namespace Core.Tests.ProtoBuf
{
    [DataContract]
    [ProtoInclude(1, typeof(Class))]
    public class Base
    {
        [DataMember(Order = 10)]
        public string BaseProperty { get; set; }
    }
}