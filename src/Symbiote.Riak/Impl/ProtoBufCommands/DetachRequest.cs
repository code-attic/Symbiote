using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBufCommands
{
    [Serializable, DataContract(Name = "RpbLink")]
    public class RiakLink
    {

        [DataMember(Order = 1, IsRequired = false, Name = "bucket")]
        public byte[] Bucket { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember(Order = 3, IsRequired = false, Name = "tag")]
        public byte[] Tag { get; set; }
    }

    public class RiakPair
    {
        [DataMember(Order = 1, IsRequired = true, Name = "key")]
        public byte[] Key { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "value")]
        public byte[] Value { get; set; }
    }

    [Serializable, DataContract(Name = "RbpContent")]
    public class RiakContent
    {
        [DataMember(Order = 1, Name = "value")]
        public byte[] Value { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "content_type")]
        public byte[] ContentType { get; set; }

        [DataMember(Order = 3, IsRequired = false, Name = "charset")]
        public byte[] Charset { get; set; }

        [DataMember(Order = 4, IsRequired = false, Name = "content_encoding")]
        public byte[] ContentEncoding { get; set; }

        [DataMember(Order = 5, IsRequired = false, Name = "vtag")]
        public byte[] Vtag { get; set; }

        [DataMember(Order = 6, Name = "links")]
        public List<RiakLink> Links { get; set; }

        [DataMember(Order = 7, IsRequired = false, Name = "last_mod")]
        public uint LastMod { get; set; }

        [DataMember(Order = 8, IsRequired = false, Name = "last_mod_usecs")]
        public uint LastModSecs { get; set; }

        [DataMember(Order = 9, Name = "usermeta")]
        public List<RiakPair> UserMetadata { get; set; }


    }

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

    [Serializable, DataContract(Name = "RpbGetResp")]
    public class GetResult
    {
        [DataMember(Order = 1, Name = "content")]
        public List<RiakContent> Content { get; set; }
    }

    
}
