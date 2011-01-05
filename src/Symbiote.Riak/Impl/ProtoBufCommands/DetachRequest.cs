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

    [Serializable, DataContract(Name = "RpbPair")]
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
    public class Delete
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
    public class Get
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

        [DataMember(Order = 2, IsRequired = false, Name = "vclock")]
        public byte[] VectorClock { get; set; }
    }

    [Serializable, DataContract(Name = "RpbGetServerInfoResp")]
    public class ServerInformation
    {
        [DataMember(Order = 1, IsRequired = false, Name = "node")]
        public byte[] Node { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "server_version")]
        public byte[] ServerVersion { get; set; }
    }

    [Serializable, DataContract(Name = "RpbListBucketResp")]
    public class BucketList
    {
        [DataMember(Order = 1, Name = "buckets")]
        public List<byte[]> Buckets { get; set; }
    }

    [Serializable, DataContract(Name = "RpbListKeysReq")]
    public class GetKeyList
    {
        [DataMember(Order = 1, IsRequired = true, Name = "bucket")]
        public byte[] Bucket { get; set; }
    }

    [Serializable, DataContract(Name = "RpbListKeysResp")]
    public class KeyList
    {
        [DataMember(Order = 1, IsRequired = true, Name = "keys")]
        public List<byte[]> Keys { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "done")]
        public bool Done { get; set; }
    }

    [Serializable, DataContract(Name = "RpbErrorResp")]
    public class Error
    {
        [DataMember(Order = 1, IsRequired = true, Name = "errmsg")]
        public byte[] Message { get; set; }

        [DataMember(Order = 2, IsRequired = true, Name = "errcode")]
        public uint Code { get; set; }
    }

    [Serializable, DataContract(Name = "RpbSetClientIdReq")]
    public class PersistClientId
    {
        [DataMember(Order = 1, IsRequired = true, Name = "client_id")]
        public byte[] ClientId { get; set; }
    }


}
