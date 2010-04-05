namespace Symbiote.Jackalope
{
    public interface IResponse
    {
        void Acknowledge();
        void Reject();

        void Reply<TReply>(TReply reply)
            where TReply : class;

        string FromQueue { get; }
        string ExchangeName { get; }
    }
}