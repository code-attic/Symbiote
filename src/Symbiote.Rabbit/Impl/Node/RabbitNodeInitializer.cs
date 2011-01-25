

using System;
using System.Collections.Concurrent;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Mesh;

namespace Symbiote.Rabbit.Impl.Node
{
    public class RabbitNodeChannelManager
        : INodeChannelManager
    {
        public INodeConfiguration NodeConfiguration { get; set; }
        public INodeHealthMonitor NodeMonitor { get; set; }
        public INodeRegistry NodeRegistry { get; set; }
        public IBus Bus { get; set; }

        public void AddNewOutgoingChannel( string channelName )
        {
            var nodeExchange = NodeConfiguration.GetNodeChannelForId( channelName );
            Bus.AddRabbitChannel(x => x
                .Direct(nodeExchange)
                .Immediate()
                .Mandatory()
                .AutoDelete()
                .PersistentDelivery() // we do want messages sticking around even if the broker dies
                );
        }

        public void InitializeChannels()
        {
            var nodeExchange = NodeConfiguration.NodeChannel;
            var nodeQueue = NodeConfiguration.NodeChannel + "Q";
            var meshExchange = NodeConfiguration.MeshChannel;

            Bus.AddRabbitChannel( x => x
                .Direct( nodeExchange )
                .Immediate()
                .Mandatory()
                .AutoDelete()
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


            //Now for the magic
            Bus.BindExchangeToQueue( meshExchange, nodeExchange, NodeConfiguration.NodeChannel );

            //Add 'self' to the node registry
            this.NodeRegistry.AddNode( NodeConfiguration.IdentityProvider.Identity );

            //Start the monitor
            if(!NodeConfiguration.AsProxy)
                NodeMonitor.Start();
        }

        public RabbitNodeChannelManager( INodeConfiguration nodeConfiguration, INodeHealthMonitor nodeMonitor, INodeRegistry nodeRegistry, IBus bus )
        {
            NodeConfiguration = nodeConfiguration;
            NodeMonitor = nodeMonitor;
            NodeRegistry = nodeRegistry;
            Bus = bus;
        }
    }
}
