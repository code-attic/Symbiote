using System.Runtime.Serialization;

namespace Symbiote.Messaging.Impl.Mesh
{
    [DataContract]
    public class NodeHealth
    {
        [DataMember(Order = 1)]
        public string NodeId { get; set; }
        [DataMember(Order = 2)]
        public decimal LoadScore { get; set; }

        public NodeHealth() {}
    }
}