using Symbiote.Core.Hashing;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeRegistry
        : INodeRegistry
    {
        protected Distributor<string> Nodes { get; set; }
        protected INodeChannelManager NodeChannelManager { get; set; }

        public void AddNode( string nodeId )
        {
            Nodes.AddNode( nodeId, nodeId );
            NodeChannelManager.AddNewOutgoingChannel( nodeId );
        }

        public string GetNodeFor<T>( T value )
        {
            return Nodes.GetNode( value );
        }

        public bool HasNode( string nodeId )
        {
            return Nodes.HasNode( nodeId );
        }

        public void RebalanceNode( string nodeId, decimal loadScore )
        {
            var total = Nodes.AliasCount * loadScore;
            Nodes.RebalanceNodeTo(nodeId, (int) total);
        }

        public void RemoveNode( string nodeId )
        {
            Nodes.RemoveNode( nodeId );
        }

        public NodeRegistry(INodeChannelManager nodeChannelManager)
        {
            NodeChannelManager = nodeChannelManager;
            Nodes = new Distributor<string>( 1000 );
        }
    }
}