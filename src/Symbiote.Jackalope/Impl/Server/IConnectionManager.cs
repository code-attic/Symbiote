using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IConnectionManager
    {
        IConnection GetConnection();
        string Protocol { get; }
    }
}