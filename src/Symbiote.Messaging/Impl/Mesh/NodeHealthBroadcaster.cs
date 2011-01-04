using System;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeHealthBroadcaster
        : INodeHealthBroadcaster
    {
        public INodeConfiguration Configuration { get; set; }
        public IBus Bus { get; set; }

        public void OnNext( NodeHealth value )
        {
            Bus.Publish( Configuration.MeshChannel, value );
        }

        public void OnError( Exception error )
        {
            // do nothing
        }

        public void OnCompleted()
        {
            // do nothing
        }

        public NodeHealthBroadcaster( INodeConfiguration configuration, IBus bus )
        {
            Configuration = configuration;
            Bus = bus;
        }
    }
}