using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbSetBucketReq" )]
    public class SetBucketProperties : RiakCommand<SetBucketProperties, BucketProperties>
    {
        [DataMember( Order = 1, Name = "bucket", IsRequired = true )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, Name = "props", IsRequired = true )]
        public BucketProperties Properties { get; set; }

        public SetBucketProperties() {}

        public SetBucketProperties( string bucket, BucketProperties properties )
        {
            Bucket = bucket.ToBytes();
            Properties = properties;
        }
    }
}