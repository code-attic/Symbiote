namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeChangeHandler :
        IHandle<NodeUp>,
        IHandle<NodeDown>,
        IHandle<NodeHealth>
    {
        protected INodeRegistry Registry { get; set; }

        public void Handle( IEnvelope<NodeUp> envelope )
        {
            Registry.AddNode( envelope.Message.NodeId );
        }

        public void Handle( IEnvelope<NodeDown> envelope )
        {
            Registry.RemoveNode(envelope.Message.NodeId);
        }

        public void Handle( IEnvelope<NodeHealth> envelope )
        {
            Registry.RebalanceNode( envelope.Message.NodeId, envelope.Message.LoadScore );
        }

        public NodeChangeHandler( INodeRegistry registry )
        {
            Registry = registry;
        }
    }
}