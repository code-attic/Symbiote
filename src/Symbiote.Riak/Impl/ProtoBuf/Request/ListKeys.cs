using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbListKeysReq" )]
    public class ListKeys
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        public ListKeys() {}

        public ListKeys( string bucket )
        {
            Bucket = bucket.ToBytes();
        }
    }
}