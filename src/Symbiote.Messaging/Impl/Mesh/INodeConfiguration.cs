namespace Symbiote.Messaging.Impl.Mesh
{
    public class NodeConfigurator
    {
        public INodeConfiguration Configuration { get; set; }


        public void Init()
        {
            Configuration.BroadcastChannel = "broadcast";
        }

        public NodeConfigurator BroadcastChannel(string broadcastTo)
        {
            Configuration.BroadcastChannel = broadcastTo;
            return this;
        }

        public NodeConfigurator( INodeConfiguration configuration )
        {
            Configuration = configuration;
        }
    }

    public interface INodeConfiguration
    {
        string BroadcastChannel { get; set; }
        string MeshChannel { get; set; }
        string NodeChannel { get; set; }
    }
}