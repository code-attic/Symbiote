using Symbiote.Jackalope.Impl;

namespace Symbiote.Jackalope.Control
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