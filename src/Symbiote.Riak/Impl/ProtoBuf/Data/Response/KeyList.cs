using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbListKeysResp")]
    public class KeyList
    {
        [DataMember(Order = 1, IsRequired = true, Name = "keys")]
        public List<byte[]> Keys { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "done")]
        public bool Done { get; set; }
    }
}