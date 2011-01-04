using System;

namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeConfigurator
    {
        public INodeConfiguration Configuration { get; set; }

        public NodeConfigurator BroadcastChannel(string meshChannelName)
        {
            Configuration.MeshChannel = meshChannelName;
            return this;
        }

        public NodeConfigurator HealthUpdateEvery( int milisecondsBetweenUpdates )
        {
            return HealthUpdateEvery(TimeSpan.FromMilliseconds(milisecondsBetweenUpdates));
        }

        public NodeConfigurator HealthUpdateEvery( TimeSpan timeBetweenUpdates )
        {
            Configuration.HealthMonitorFrequency = timeBetweenUpdates;
            return this;
        }

        public NodeConfigurator( INodeConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}