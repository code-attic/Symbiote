using RabbitMQ.Client;

namespace Symbiote.Jackalope.Impl
{
    public interface IMessageDelivery
    {
        void Acknowledge();
        void Reject();
        void Reply<TReply>(TReply reply)
            where TReply : class;
        string Exchange { get; }
        string Queue { get; }
        IBasicProperties Details { get; }
        bool IsReply { get; }
        bool Redelivered { get; }
        string RoutingKey { get; }
    }
}