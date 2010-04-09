using RabbitMQ.Client;

namespace Symbiote.Jackalope
{
    public interface IRespond
    {
        void Acknowledge();
        void Reject();
        void Reply<TReply>(TReply reply)
            where TReply : class;
    }
}