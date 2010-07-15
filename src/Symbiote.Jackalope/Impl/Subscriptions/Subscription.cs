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
        }
    }
}