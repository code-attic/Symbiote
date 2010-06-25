namespace Symbiote.Jackalope.Impl.Channel
{
    public interface IChannelProxyFactory
    {
        IChannelProxy GetProxyForQueue(string queueName);
        IChannelProxy GetProxyForExchange(string exchangeName);
    }
}