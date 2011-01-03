

using System;
using System.Collections.Concurrent;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Symbiote.Rabbit.Impl.Node
{
    public class RabbitNodeInitializer
        : IInitializeNode
    {
        public INodeConfiguration NodeConfiguration { get; set; }
        public INodeHealthMonitor NodeMonitor { get; set; }
        public INodeRegistry NodeRegistry { get; set; }
        public IBus Bus { get; set; }

        public void InitializeChannels()
        {
            var nodeExchange = NodeConfiguration.NodeChannel;
            var nodeQueue = NodeConfiguration.NodeChannel + "Q";
            var meshExchange = NodeConfiguration.MeshChannel;
            var meshQueue = NodeConfiguration.MeshChannel + "Q";

            Bus.AddRabbitChannel( x => x
                .Direct( nodeExchange )
                .Immediate()
                .Mandatory()
                .PersistentDelivery() // we do want messages sticking around even if the broker dies
                );

            
            Bus.AddRabbitQueue( x => x
                .QueueName( nodeQueue )
                .ExchangeName( nodeExchange )
                .Durable() // we do want messages sticking around even if the broker dies
                .AutoDelete() // we don't want queues lingering after the node is gone
                .StartSubscription() );

            
            Bus.AddRabbitChannel( x => x
                .Fanout( meshExchange)
                .Durable()
                );

            
            Bus.AddRabbitQueue( x => x
                .QueueName( meshQueue)
                .ExchangeName( meshExchange)
                .Durable() // we do want messages sticking around even if the broker dies
                .AutoDelete() // we don't want queues lingering after the node is gone
                .StartSubscription() );

            //Now for the magic
            Bus.BindExchangeToQueue( meshExchange, nodeExchange, NodeConfiguration.NodeChannel );

            //Add 'self' to the node registry
            this.NodeRegistry.AddNode( NodeConfiguration.IdentityProvider.Identity );

            //Start the monitor
            NodeMonitor.Start();
        }

        public RabbitNodeInitializer( INodeConfiguration nodeConfiguration, INodeHealthMonitor nodeMonitor, INodeRegistry nodeRegistry, IBus bus )
        {
            NodeConfiguration = nodeConfiguration;
            NodeMonitor = nodeMonitor;
            NodeRegistry = nodeRegistry;
            Bus = bus;
        }
    }
}
