using System;
using Symbiote.Core.Impl.Futures;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class Node
        : INode
    {
        public INodeRegistry Nodes { get; set; }
        public IBus Bus { get; set; }
        public INodeConfiguration Configuration { get; set; }

        public void Publish<T>( T message )
        {
            Publish( message, x => { } );
        }

        public void Publish<T>( T message, Action<IEnvelope> modifyEnvelope )
        {
            var channelName = GetChannelForMessage( message );
            Bus.Publish( channelName, message, modifyEnvelope );
        }

        public Future<R> Request<T, R>( T message )
        {
            return Request<T, R>( message, x => { } );
        }

        public Future<R> Request<T, R>( T message, Action<IEnvelope> modifyEnvelope )
        {
            var channelName = GetChannelForMessage(message);
            return Bus.Request<T, R>( channelName, message, modifyEnvelope );
        }

        public string GetChannelForMessage<T>(T message)
        {
            return Configuration.GetNodeChannelForId(Nodes.GetNodeFor( message ));
        }

        public Node( INodeRegistry nodes, IBus bus, INodeConfiguration configuration )
        {
            Nodes = nodes;
            Bus = bus;
            Configuration = configuration;
        }
    }
}