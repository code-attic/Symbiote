namespace Symbiote.Net
{
    public interface ISocketConfigurator
    {
        ISocketConfigurator ListenOn( int port );
    }
}