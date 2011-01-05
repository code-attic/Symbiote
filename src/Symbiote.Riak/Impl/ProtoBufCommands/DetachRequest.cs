using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBufCommands
{
    [Serializable, DataContract(Name = "RpbDelReq")]
    public class DeleteCommand
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )] 
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "rw")]
        public uint SuccessfulDeletionCount { get; set; }
    }

    [Serializable, DataContract(Name = "RpbGetClientIdResp")]
    public class FoundClient
    {
        [DataMember(Order = 1, IsRequired = true, Name="client_id")]
        public byte[] ClientId { get; set; }
    }

    [Serializable, DataContract(Name = "RpbGetReq")]
    public class GetCommand
    {
        [DataMember(Order = 1, IsRequired = true, Name = "bucket")] 
        public byte[] Bucket { get; set; }

        [DataMember(Order = 2, IsRequired = true, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember(Order = 3, IsRequired = false, Name = "r")]
        public uint ReadValue { get; set; }
    }



    
}
