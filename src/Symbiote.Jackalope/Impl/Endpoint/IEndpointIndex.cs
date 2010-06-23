namespace Symbiote.Jackalope.Impl
{
    public interface IEndpointIndex
    {
        IEndPoint GetEndpointByExchange(string exchangeName);
        IEndPoint GetEndpointByQueue(string queueName);
        void AddEndpoint(IEndPoint endpoint);
    }
}