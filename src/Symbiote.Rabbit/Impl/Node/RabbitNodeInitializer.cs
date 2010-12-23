

using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Symbiote.Rabbit.Impl.Node
{
    public class RabbitNodeInitializer
        : IInitializeNode
    {
        public INodeConfiguration NodeConfiguration { get; set; }
        public IBus Bus { get; set; }

        public void InitializeChannels()
        {
            Bus.AddRabbitChannel( x => x
                .Direct( NodeConfiguration.NodeChannel )
                .Immediate()
                .Mandatory()
                .PersistentDelivery() // we do want messages sticking around even if the broker dies
                );

            Bus.AddRabbitQueue( x => x
                .QueueName( NodeConfiguration.NodeChannel )
                .ExchangeName( NodeConfiguration.NodeChannel )
                .Durable() // we do want messages sticking around even if the broker dies
                .AutoDelete() // we don't want queues lingering after the node is gone
                .StartSubscription() );

            Bus.AddRabbitChannel( x => x
                .Fanout( NodeConfiguration.MeshChannel )
                .Durable()
                );

            Bus.AddRabbitQueue( x => x
                .QueueName( NodeConfiguration.MeshChannel )
                .ExchangeName( NodeConfiguration.MeshChannel )
                .Durable() // we do want messages sticking around even if the broker dies
                .AutoDelete() // we don't want queues lingering after the node is gone
                .StartSubscription() );
        }

        public RabbitNodeInitializer( INodeConfiguration nodeConfiguration, IBus bus )
        {
            NodeConfiguration = nodeConfiguration;
            Bus = bus;
        }
    }
}
