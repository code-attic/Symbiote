using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbGetClientIdResp")]
    public class Client
    {
        [DataMember(Order = 1, IsRequired = true, Name="client_id")]
        public byte[] ClientId { get; set; }
    }
}