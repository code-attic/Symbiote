/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private ConcurrentDictionary<string, ISubscription> _subscriptions = new ConcurrentDictionary<string, ISubscription>();

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

        public ISubscription EnsureSubscriptionIsRunning(string queueName)
        {
            ISubscription subscription = null;
            if(!_subscriptions.TryGetValue(queueName, out subscription))
            {
                subscription = AddSubscription(queueName);
            }
            if (subscription.Stopped || subscription.Stopping)
                subscription.Start();
            return subscription;
        }
    }
}