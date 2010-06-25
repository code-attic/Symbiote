using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl.Server
{
    public interface IConnectionProvider
    {
        IConnection GetActiveConnection();
    }
}