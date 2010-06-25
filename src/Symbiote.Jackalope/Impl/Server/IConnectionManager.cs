using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl.Server
{
    public interface IConnectionManager
    {
        IConnection GetConnection();
        string Protocol { get; }
    }
}