namespace Symbiote.Riak.Impl
{
    public class RiakNode
    {
        public string NodeAddress { get; set; }
        public int ProtoBufPort { get; set; }
        public int HttpPort { get; set; }

        public RiakNode Address(string address)
        {
            NodeAddress = address;
            return this;
        }

        public RiakNode ForHttp(int httpPort)
        {
            HttpPort = httpPort;
            return this;
        }

        public RiakNode ForProtocolBufferPort(int protocolBufferPort)
        {
            ProtoBufPort = protocolBufferPort;
            return this;
        }

        public RiakNode()
        {
            NodeAddress = "127.0.0.1";
            ProtoBufPort = 8087;
            HttpPort = 8091;
        }
    }
}