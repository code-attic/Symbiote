using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbPutReq" )]
    public class Persist : RiakCommand<Persist, Persisted>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key" )]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "vclock" )]
        public byte[] VectorClock { get; set; }

        [DataMember( Order = 4, IsRequired = true, Name = "content" )]
        public RiakContent Content { get; set; }

        [DataMember( Order = 5, IsRequired = false, Name = "w" )]
        public uint Write { get; set; }

        [DataMember( Order = 6, IsRequired = false, Name = "dw" )]
        public uint Dw { get; set; }

        [DataMember( Order = 7, IsRequired = false, Name = "return_body" )]
        public bool ReturnBody { get; set; }

        public Persist() {}

        public Persist( string bucket, string key, string vectorClock, RiakContent content, uint write, uint dw, bool returnBody )
        {
            Bucket = bucket.ToBytes();
            Key = key.ToBytes();
            VectorClock = vectorClock.ToBytes();
            Content = content;
            Write = write;
            Dw = dw;
            ReturnBody = returnBody;
        }
    }
}