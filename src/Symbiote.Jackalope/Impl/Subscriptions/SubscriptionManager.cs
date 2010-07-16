using System;
using System.Collections.Concurrent;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private ConcurrentDictionary<string, ISubscription> _subscriptions = new ConcurrentDictionary<string, ISubscription>();

        public IObservable<Envelope> this[string queueName]
        {
            get
            {
                EnsureSubscriptionIsRunning(queueName);
                return _subscriptions[queueName].MessageStream;
            }
        }

        public void StartAllSubscriptions()
        {
            _subscriptions
                .ForEach(x =>
                             {
                                 if (x.Value.Stopped || x.Value.Stopping)
                                     x.Value.Start();
                             });
        }

        public void StopAllSubscriptions()
        {
            _subscriptions
                .ForEach(x =>
                             {
                                 if (x.Value.Started || x.Value.Starting)
                                 {
                                     x.Value.Stop();
                                 }
                             });
        }

        public void StartSubscription(string queueName)
        {
            ISubscription subscription = null; 
            if(_subscriptions.TryGetValue(queueName, out subscription))
                if(subscription.Stopped || subscription.Stopping)
                    subscription.Start();
        }

        public void StopSubscription(string queueName)
        {
            ISubscription subscription = null;
            if (_subscriptions.TryRemove(queueName, out subscription))
                if (subscription.Started || subscription.Starting)
                {
                    subscription.Dispose();
                }
        }

        public ISubscription AddSubscription(string queueName)
        {
            ISubscription subscription = null;

            if(!_subscriptions.TryGetValue(queueName, out subscription))
            {
                subscription = ServiceLocator
                    .Current
                    .GetInstance<ISubscription>();
                subscription.QueueName = queueName;
                _subscriptions[queueName] = subscription;
                subscription.Start();
            }
            return subscription;
        }

        protected void EnsureSubscriptionIsRunning(string queueName)
        {
            ISubscription subscription = null;
            if(!_subscriptions.TryGetValue(queueName, out subscription))
            {
                subscription = AddSubscription(queueName);
            }
            if (subscription.Stopped || subscription.Stopping)
                subscription.Start();
        }
    }
}