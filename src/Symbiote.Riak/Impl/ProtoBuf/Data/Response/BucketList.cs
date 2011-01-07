using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbListBucketResp")]
    public class BucketList
    {
        [DataMember(Order = 1, Name = "buckets")]
        public List<byte[]> Buckets { get; set; }
    }
}