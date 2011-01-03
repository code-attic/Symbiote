using System;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeConfiguration
        : INodeConfiguration
    {
        protected readonly string MESH_FORMAT = "mesh.{0}";
        protected readonly string NODE_FORMAT = "node.{0}";
        public INodeIdentityProvider IdentityProvider { get; set; }
        public TimeSpan HealthMonitorFrequency { get; set; }

        public string MeshChannel { get; set; }
        public string NodeChannel { get; set; }
        public string BroadcastChannel { get; set; }

        public NodeConfiguration( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
            MeshChannel = MESH_FORMAT.AsFormat( identityProvider.Identity );
            NodeChannel = NODE_FORMAT.AsFormat( identityProvider.Identity );
        }
    }
}