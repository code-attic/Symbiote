using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbPair")]
    public class RiakPair
    {
        [DataMember(Order = 1, IsRequired = true, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "value")]
        public byte[] Value { get; set; }
    }
}