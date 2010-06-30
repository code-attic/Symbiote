using System;
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using Symbiote.Jackalope.Impl.Dispatch;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class Subscription : ISubscription
    {
        private string _queueName;
        private IQueueObserver _observer;
        private IDispatchMessages _dispatcher;

        public IObservable<Envelope> MessageStream
        {
            get { return _observer; }
        }

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

        public Subscription(string queueName)
        {
            _queueName = queueName;
            Start();
        }

        protected void InitializeBrokerTask()
        {
            _observer = ObjectFactory.GetInstance<IQueueObserver>();
            _dispatcher = ObjectFactory.GetInstance<IDispatchMessages>();
            _observer.Subscribe(_dispatcher);
            _observer.Start(_queueName);
            _observer
                .TimeInterval()
                .Subscribe(x =>
                               {
                                   var interval = x.Interval.Milliseconds;
                                   if (interval == _observer.SleepFor && _observer.SleepFor > 0)
                                   {
                                       _observer.SleepFor -= 5;
                                   }
                                   else if (interval > _observer.SleepFor)
                                   {
                                       _observer.SleepFor = interval;
                                   }
                                   else
                                   {
                                       _observer.SleepFor = 0;
                                   }
                               });
        }
    }
}