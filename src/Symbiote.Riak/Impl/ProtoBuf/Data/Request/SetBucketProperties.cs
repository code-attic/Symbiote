using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Data.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Request
{
    [Serializable, DataContract(Name = "RpbSetBucketReq")]
    public class SetBucketProperties
    {
        [DataMember(Order = 1, Name = "bucket", IsRequired = true)]
        public byte[] Bucket { get; set; }

        [DataMember(Order = 2, Name = "props", IsRequired = true)]
        public BucketProperties Properties { get; set; }

        public SetBucketProperties() {}

        public SetBucketProperties( string bucket, BucketProperties properties )
        {
            Bucket = bucket.ToBytes();
            Properties = properties;
        }
    }
}