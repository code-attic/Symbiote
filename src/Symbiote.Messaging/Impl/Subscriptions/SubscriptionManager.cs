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
using System.Collections.Generic;
using System.Threading;
using Symbiote.Messaging.Impl.Dispatch;

namespace Symbiote.Messaging.Impl.Subscriptions
{
    public class SubscriptionManager :
        ISubscriptionManager,
        IObserver<IEnvelope>
    {
        protected ReaderWriterLockSlim ObserverLock { get; set; }
        protected HashSet<IObserver<IEnvelope>> Observers { get; set; }
        protected Dictionary<string, ISubscription> Subscriptions { get; set; }
        protected IDispatcher Dispatcher { get; set; }

        public IDisposable Subscribe(IObserver<IEnvelope> observer)
        {
            ObserverLock.EnterWriteLock();
            Observers.Add(observer);
            ObserverLock.ExitWriteLock();
            return new ObserverToken();
        }

        public void Dispose()
        {
            Observers.Clear();
            Observers = null;
            ObserverLock.Dispose();
        }

        public void AddSubscription(ISubscription subscription)
        {
            subscription.Subscribe(this);
            Subscriptions[subscription.Name] = subscription;
            subscription.Start();
        }

        public void StartSubscription(string subscriptionName)
        {
            var subscription = Subscriptions[subscriptionName];
            subscription.Start();
        }

        public void StopSubscription(string subscriptionName)
        {
            var subscription = Subscriptions[subscriptionName];
            subscription.Stop();
        }

        public void OnNext(IEnvelope value)
        {
            Dispatcher.Send(value);
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnCompleted()
        {
            
        }

        public SubscriptionManager(IDispatcher dispatcher)
        {
            ObserverLock = new ReaderWriterLockSlim();
            Observers = new HashSet<IObserver<IEnvelope>>();
            Subscriptions = new Dictionary<string,ISubscription>();
            Dispatcher = dispatcher;
        }
    }
}