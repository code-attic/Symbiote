using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IConnectionProvider
    {
        IConnection GetActiveConnection();
    }
}