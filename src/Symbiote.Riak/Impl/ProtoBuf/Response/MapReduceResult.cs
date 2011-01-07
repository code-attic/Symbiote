using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Response
{
    [Serializable, DataContract( Name = "RpbMapRedResp" )]
    public class MapReduceResult
    {
        [DataMember( Order = 1, Name = "phase", IsRequired = false )]
        public uint Phase { get; set; }

        [DataMember( Order = 2, Name = "response", IsRequired = false )]
        public byte[] Response { get; set; }

        [DataMember( Order = 3, Name = "done", IsRequired = false )]
        public bool Done { get; set; }
    }
}