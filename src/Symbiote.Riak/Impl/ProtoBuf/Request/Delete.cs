using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbDelReq" )]
    public class Delete : RiakCommand<Delete, Deleted>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key" )]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "rw" )]
        public uint SuccessfulDeletionCount { get; set; }

        public Delete() {}

        public Delete( string bucket, string key, uint minimum )
        {
            Bucket = bucket.ToBytes();
            Key = key.ToBytes();
            SuccessfulDeletionCount = minimum.ToggleEndianicity();
        }
    }
}