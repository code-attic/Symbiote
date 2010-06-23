using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl
{
    public interface IEndPoint
    {
        IAmqpEndpointConfiguration EndpointConfiguration { get; }
        IEndPoint QueueName(string queueName);
        IEndPoint RoutingKeys(params string[] routingKeys);
        IEndPoint Exchange(string exchangeName, ExchangeType exchange);
        IEndPoint Durable();
        IEndPoint Exclusive();
        IEndPoint Passive();
        IEndPoint AutoDelete();
        IEndPoint Internal();
        IEndPoint NoWait();
        IEndPoint NoAck();
        IEndPoint PersistentDelivery();
        bool Initialized { get; set; }
    }
}