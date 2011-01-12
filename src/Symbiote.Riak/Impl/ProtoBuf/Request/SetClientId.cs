using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbSetClientIdReq" )]
    public class SetClientId : RiakCommand<SetClientId, ClientIdSet>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "client_id" )]
        public byte[] ClientId { get; set; }

        public SetClientId() {}

        public SetClientId( string clientId )
        {
            ClientId = clientId.ToBytes();
        }
    }
}