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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Jackalope.Impl.Dispatch;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class Subscription : ISubscription
    {
        private IQueueObserver _observer;
        private IDispatchMessages _dispatcher;

        public IObservable<Envelope> MessageStream
        {
            get { return _observer; }
        }

        public string QueueName { get; set; }
        public bool Starting { get; private set; }
        public bool Started { get; private set; }
        public bool Stopping { get; private set; }
        public bool Stopped { get; private set; }
        
        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Stopped = Stopping = false;
            if(!Started || !Starting)
            {
                Starting = true;
                InitializeBrokerTask();
                Starting = false;
                Started = true;
            }
        }

        public void Stop()
        {
            Started = Starting = false;
            _observer.OnShutdown -= OnObserverShutdown;
            Stopping = true;
            _observer.Stop();
            _observer = null;
            Stopping = false;
            Stopped = true;
        }

        protected void InitializeBrokerTask()
        {
            _observer = ServiceLocator.Current.GetInstance<IQueueObserver>();
            _dispatcher = ServiceLocator.Current.GetInstance<IDispatchMessages>();
            _observer.Subscribe(_dispatcher);
            _observer.Start(QueueName);
            _observer.OnShutdown += OnObserverShutdown;
        }

        protected void OnObserverShutdown(IQueueObserver observer)
        {
            if(!Stopping && !Stopped)
            {
                observer.Start(QueueName);
            }
        }
    }
}