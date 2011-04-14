namespace Symbiote.Net
{
    public class SocketConfigurator
        : ISocketConfigurator
    {
        public SocketConfiguration Configuration { get; set; }

        public ISocketConfigurator ListenOn( int port )
        {
            Configuration.Port = port;
            return this;
        }

        public SocketConfigurator()
        {
            Configuration = new SocketConfiguration();
        }
    }
}