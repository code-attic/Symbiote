using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Response
{
    [Serializable, DataContract( Name = "RpbErrorResp" )]
    public class Error
    {
        [DataMember( Order = 1, IsRequired = true, Name = "errmsg" )]
        public byte[] Message { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "errcode" )]
        public uint Code { get; set; }
    }
}