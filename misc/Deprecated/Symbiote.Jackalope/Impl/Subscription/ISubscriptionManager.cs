namespace Symbiote.Jackalope.Impl
{
    public interface ISubscriptionManager : IQueueStreamCollection
    {
        void StartAllSubscriptions();
        void StopAllSubscriptions();
        void StartSubscription(string queueName);
        void StopSubscription(string queueName);
        ISubscription AddSubscription(string queueName);
    }
}