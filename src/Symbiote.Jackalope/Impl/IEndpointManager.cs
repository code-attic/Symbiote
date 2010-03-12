namespace Symbiote.Jackalope.Impl
{
    public interface IEndpointManager
    {
        IEndPoint GetEndpointByExchange(string exchangeName);
        IEndPoint GetEndpointByQueue(string queueName);
        void AddEndpoint(IEndPoint endpoint);
    }
}