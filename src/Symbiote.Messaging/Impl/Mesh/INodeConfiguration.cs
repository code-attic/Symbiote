using System;

namespace Symbiote.Messaging.Impl.Mesh
{
    public interface INodeConfiguration
    {
        bool AsProxy { get; set; }
        string MeshChannel { get; set; }
        string NodeChannel { get; set; }
        string GetNodeChannelForId( string nodeId );
        TimeSpan HealthMonitorFrequency { get; set; }
        INodeIdentityProvider IdentityProvider { get; set; }
    }
}