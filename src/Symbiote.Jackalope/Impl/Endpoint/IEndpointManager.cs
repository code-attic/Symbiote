namespace Symbiote.Jackalope.Impl.Endpoint
{
    public interface IEndpointManager : IEndpointIndex
    {
        void ConfigureEndpoint(IEndPoint endpoint);
        void BindQueue(string exchangeName, string queueName, params string[] routingKeys);
    }
}