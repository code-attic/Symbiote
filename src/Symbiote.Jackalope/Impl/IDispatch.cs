using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Symbiote.Jackalope.Impl
{
    public interface IDispatch
    {
        bool CanHandle(object payload);
        void Dispatch(object payload, IChannelProxy proxy, BasicDeliverEventArgs args);
        void Dispatch(object payload, IChannelProxy proxy, BasicGetResult result);
    }

    public interface IDispatch<TMessage> : IDispatch
    {
    }
}