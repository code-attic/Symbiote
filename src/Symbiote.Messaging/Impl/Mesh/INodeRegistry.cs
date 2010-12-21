using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Hashing;
using Symbiote.Core.Utility;

namespace Symbiote.Messaging.Impl.Node
{
    public interface INodeConfiguration
    {
        string BroadcastChannel { get; set; }
    }

    public class NodeConfiguration
        : INodeConfiguration
    {
        public string BroadcastChannel { get; set; }
    }

    public interface INode
    {
        void Publish<T>( T message );
        void Publish<T>( T message, Action<IEnvelope> modifyEnvelope);
        Future<R> Request<T, R>( T message );
        Future<R> Request<T, R>(T message, Action<IEnvelope> modifyEnvelope);
    }

    public interface INodeIdentityProvider
    {
        
    }

    public interface INodeChannelProvider
    {
        
    }

    public class Node
        : INode
    {
        public INodeRegistry Nodes { get; set; }
        public IBus Bus { get; set; }
        public INodeConfiguration Configuration { get; set; }
        public INodeIdentityProvider IdentityProvider { get; set; }
        public INodeChannelProvider ChannelProvider { get; set; }

        public void Initialize()
        {
            
        }

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
            return Nodes.GetNodeFor( message );
        }

        public Node( INodeRegistry nodes, IBus bus, INodeConfiguration configuration, INodeIdentityProvider identityProvider, INodeChannelProvider channelProvider )
        {
            Nodes = nodes;
            Bus = bus;
            Configuration = configuration;
            IdentityProvider = identityProvider;
            ChannelProvider = channelProvider;
            Initialize();
        }
    }

    public class NodeUp
    {
        public string NodeId { get; set; }
    }

    public class NodeDown
    {
        public string NodeId { get; set; }
    }

    public class NodeHealth
    {
        public string NodeId { get; set; }
        public decimal LoadScore { get; set; }
    }

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
            
        }

        public NodeChangeHandler( INodeRegistry registry )
        {
            Registry = registry;
        }
    }

    public interface INodeRegistry
    {
        void AddNode( string NodeId );
        void RemoveNode( string NodeId );
        string GetNodeFor<T>( T value );
    }

    public class NodeRegistry
        : INodeRegistry
    {
        protected Distributor<string> Nodes { get; set; }

        public void AddNode( string NodeId )
        {
            Nodes.AddNode( NodeId, NodeId );
        }

        public void RemoveNode( string NodeId )
        {
            Nodes.RemoveNode( NodeId );
            
        }

        public string GetNodeFor<T>( T value )
        {
            return Nodes.GetNode( value );
        }

        public NodeRegistry()
        {
            Nodes = new Distributor<string>( 1000 );
        }
    }
}
