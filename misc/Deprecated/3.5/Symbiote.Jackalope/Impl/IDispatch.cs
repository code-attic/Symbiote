using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Symbiote.Jackalope.Impl
{
    public interface IDispatch
    {
        bool CanHandle(object payload);
        void Dispatch(object payload, IChannelProxy proxy, BasicDeliverEventArgs args);
        void Dispatch(object payload, IChannelProxy proxy, BasicGetResult result);
        object Dispatch(Envelope envelope);
    }

    public interface IDispatch<TMessage> : IDispatch
    {
    }
}