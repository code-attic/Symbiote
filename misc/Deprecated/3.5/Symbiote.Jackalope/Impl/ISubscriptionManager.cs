namespace Symbiote.Jackalope.Impl
{
    public interface ISubscriptionManager
    {
        void StartAllSubscriptions();
        void StopAllSubscriptions();
        void StartSubscription(string queueName);
        void StopSubscription(string queueName);
        ISubscription AddSubscription(string queueName, int threads);
    }
}