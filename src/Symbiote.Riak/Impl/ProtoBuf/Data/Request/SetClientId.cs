using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Request
{
    [Serializable, DataContract(Name = "RpbSetClientIdReq")]
    public class SetClientId
    {
        [DataMember(Order = 1, IsRequired = true, Name = "client_id")]
        public byte[] ClientId { get; set; }

        public SetClientId() {}

        public SetClientId( string clientId )
        {
            ClientId = clientId.ToBytes();
        }
    }
}