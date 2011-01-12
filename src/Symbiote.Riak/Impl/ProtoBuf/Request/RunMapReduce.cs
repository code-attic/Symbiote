using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbMapRedReq" )]
    public class RunMapReduce : RiakCommand<RunMapReduce, MapReduceResult>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "request" )]
        public byte[] Request { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "content_type" )]
        public byte[] ContentType { get; set; }

        public RunMapReduce() {}

        public RunMapReduce( string request, string contentType )
        {
            Request = request.ToBytes();
            ContentType = contentType.ToBytes();
        }
    }
}