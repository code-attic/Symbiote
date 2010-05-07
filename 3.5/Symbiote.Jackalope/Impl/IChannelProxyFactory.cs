namespace Symbiote.Jackalope.Impl
{
    public interface IChannelProxyFactory
    {
        IChannelProxy GetProxyForQueue(string queueName);
        IChannelProxy GetProxyForExchange(string exchangeName);
    }
}