using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private Dictionary<string, ISubscription> _subscriptions = new Dictionary<string, ISubscription>();

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
            if (_subscriptions.TryGetValue(queueName, out subscription))
                if (subscription.Started || subscription.Starting)
                {
                    _subscriptions.Remove(queueName);
                    subscription.Dispose();
                }
        }

        public ISubscription AddSubscription(string queueName, int threads)
        {
            ISubscription subscription = null;

            if(!_subscriptions.TryGetValue(queueName, out subscription))
            {
                subscription = ObjectFactory
                    .With<string>(queueName)
                    .GetInstance<ISubscription>();
                    _subscriptions[queueName] = subscription;
            }
            return subscription;
        }
    }
}