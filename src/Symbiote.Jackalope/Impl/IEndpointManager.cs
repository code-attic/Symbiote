using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IEndpointManager
    {
        void ConfigureEndpoint(IEndPoint endpoint);
        void BindQueue(string exchangeName, string queueName, params string[] routingKeys);
        void BindQueue(IModel channel, string exchangeName, string queueName, params string[] routingKeys);
    }
}