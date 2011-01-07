using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Response
{
    [Serializable, DataContract( Name = "RpbGetServerInfoResp" )]
    public class ServerInformation
    {
        [DataMember( Order = 1, IsRequired = false, Name = "node" )]
        public byte[] Node { get; set; }

        [DataMember( Order = 2, IsRequired = false, Name = "server_version" )]
        public byte[] ServerVersion { get; set; }
    }
}