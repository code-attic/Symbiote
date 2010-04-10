namespace Symbiote.Jackalope.Impl
{
    public interface IBroker
    {
        void Start(string queueName);
        void Stop();
    }
}