namespace Symbiote.Jackalope.Impl.Channel
{
    public interface IChannelProxyFactory
    {
        IChannelProxy GetProxyForQueue(string queueName);
        IChannelProxy GetProxyForExchange(string exchangeName);
        IChannelProxy GetProxyForQueue<T>(string queueName, T id);
        IChannelProxy GetProxyForExchange<T>(string exchangeName, T id);
    }
}