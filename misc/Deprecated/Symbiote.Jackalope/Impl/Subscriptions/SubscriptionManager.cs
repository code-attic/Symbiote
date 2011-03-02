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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private ConcurrentDictionary<string, List<ISubscription>> _subscriptions = new ConcurrentDictionary<string, List<ISubscription>>();
        private int ActualSubscribers { get; set; }

        public void StartAllSubscriptions()
        {
            _subscriptions
                .SelectMany(x => x.Value)
                .ForEach(x =>
                {
                    if (x.Stopped || x.Stopping)
                        x.Start();
                });
        }

        public void StopAllSubscriptions()
        {
            _subscriptions
                .SelectMany(x => x.Value)
                .ForEach(x =>
                {
                    if (x.Started || x.Starting)
                        x.Stop();
                });
        }

        public void StartSubscription(string queueName)
        {
            ApplyCommand(queueName, x => x.Stopped || x.Stopping, x => x.Start());
        }

        public void StopSubscription(string queueName)
        {
            ApplyCommand(queueName, x => x.Started || x.Starting, x => x.Stop());
        }

        public void ApplyCommand(string queueName, Predicate<ISubscription> precondition, Action<ISubscription> command)
        {
            List<ISubscription> subscriptions = null;
            if (_subscriptions.TryGetValue(queueName, out subscriptions))
            {
                subscriptions
                    .Where(x => precondition(x))
                    .ForEach(command);
            }
        }

        public IEnumerable<ISubscription> AddSubscription(string queueName)
        {
            List<ISubscription> subscriptions = null;

            if(!_subscriptions.TryGetValue(queueName, out subscriptions))
            {
                subscriptions = new List<ISubscription>();
                for (int i = 0; i < ActualSubscribers; i++ )
                {
                    var subscription = ServiceLocator
                        .Current
                        .GetInstance<ISubscription>();
                    subscription.QueueName = queueName;
                    subscription.Start();
                    subscriptions.Add(subscription);
                }
                _subscriptions[queueName] = subscriptions;
            }
            return subscriptions;
        }

        public IEnumerable<ISubscription> EnsureSubscriptionIsRunning(string queueName)
        {
            List<ISubscription> subscriptions = null;
            if(!_subscriptions.TryGetValue(queueName, out subscriptions))
            {
                subscriptions = AddSubscription(queueName).ToList();
            }
            return subscriptions
                .Where(x => x.Stopped || x.Stopping)
                .Select(x =>
                            {
                                x.Start();
                                return x;
                            });
        }

        public SubscriptionManager()
        {
            ActualSubscribers = 1;
        }
    }
}