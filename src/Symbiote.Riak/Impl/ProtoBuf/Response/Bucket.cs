using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Response
{
    [Serializable, DataContract( Name = "RpbGetBucketResp" )]
    public class Bucket
    {
        [DataMember( Order = 1, IsRequired = true, Name = "props" )]
        public BucketProperties Properties { get; set; }
    }
}