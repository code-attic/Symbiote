using System.Runtime.Serialization;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [DataContract]
    public class GetServerInfo : RiakCommand<GetServerInfo, ServerInformation>
    {
    }
}