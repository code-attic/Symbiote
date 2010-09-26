using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl.Server
{
    public interface IConnectionManager
    {
        IConnection GetConnection();
        IConnection GetConnection(string brokerName);
        IConnection GetConnection<T>(T id);
        IConnection GetConnection<T>(T id, string brokerName);
        string Protocol { get; }
    }
}