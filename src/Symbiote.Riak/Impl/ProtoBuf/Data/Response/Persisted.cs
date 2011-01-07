using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbPutResp")]
    public class Persisted
    {
        [DataMember(Order = 1, Name = "content")]
        public List<RiakContent> Content { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "vclock")]
        public byte[] VectorClock { get; set; }
    }
}