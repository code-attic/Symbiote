using System;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeConfiguration
        : INodeConfiguration
    {
        protected readonly string NODE_FORMAT = "node.{0}";
        public INodeIdentityProvider IdentityProvider { get; set; }
        public TimeSpan HealthMonitorFrequency { get; set; }

        public string MeshChannel { get; set; }
        public string NodeChannel { get; set; }

        public string GetNodeChannelForId( string nodeId )
        {
            return NODE_FORMAT.AsFormat( nodeId );
        }

        public NodeConfiguration( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
            MeshChannel = "mesh";
            NodeChannel = NODE_FORMAT.AsFormat( identityProvider.Identity );
        }
    }
}