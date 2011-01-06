using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbLink")]
    public class RiakLink
    {

        [DataMember(Order = 1, IsRequired = false, Name = "bucket")]
        public byte[] Bucket { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember(Order = 3, IsRequired = false, Name = "tag")]
        public byte[] Tag { get; set; }
    }
}
