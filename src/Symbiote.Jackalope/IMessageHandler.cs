namespace Symbiote.Jackalope
{
    public interface IMessageHandler
    {
        
    }

    public interface IMessageHandler<TBody> : IMessageHandler
        where TBody : class
    {
        void Process(TBody message, IRespond respond);
    }
}