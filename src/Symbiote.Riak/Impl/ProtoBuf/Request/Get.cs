using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbGetReq" )]
    public class Get
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key" )]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "r" )]
        public uint MinimumNodes { get; set; }

        public Get() {}

        public Get( string bucket, string key, uint readFrom )
        {
            Bucket = bucket.ToBytes();
            Key = key.ToBytes();
            MinimumNodes = readFrom;
        }
    }
}