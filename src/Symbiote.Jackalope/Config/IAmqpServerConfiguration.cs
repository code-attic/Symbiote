using RabbitMQ.Client;

namespace Symbiote.Jackalope.Config
{
    public interface IAmqpServerConfiguration
    {
        string Broker { get; set; }
        string Protocol { get; set; }
        string Address { get; set; }
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string VirtualHost { get; set; }
        IConnection GetConnection();
    }
}