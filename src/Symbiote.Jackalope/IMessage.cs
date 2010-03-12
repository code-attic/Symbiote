namespace Symbiote.Jackalope
{
    public interface IMessage<T> where T : class
    {
        T MessageBody { get; }
        void Acknowledge();
        void Reject();
        void Reply<TReply>(TReply reply) where TReply : class;
    }
}