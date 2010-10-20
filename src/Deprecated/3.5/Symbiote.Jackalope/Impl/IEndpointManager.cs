using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IEndpointManager : IEndpointIndex
    {
        void ConfigureEndpoint(IEndPoint endpoint);
        void BindQueue(string exchangeName, string queueName, params string[] routingKeys);
    }
}