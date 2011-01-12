using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbListKeysReq" )]
    public class ListKeys : RiakCommand<ListKeys, KeyList>
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