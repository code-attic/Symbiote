using System;

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
            try
            {
                var nodeId = envelope.Message.NodeId;
                if( !Registry.HasNode( nodeId ) )
                    Registry.AddNode( nodeId );

                Registry.RebalanceNode( nodeId, envelope.Message.LoadScore );
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
            }
        }

        public NodeChangeHandler( INodeRegistry registry )
        {
            Registry = registry;
        }
    }
}