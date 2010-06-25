using System;
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using Symbiote.Jackalope.Impl.Dispatch;

namespace Symbiote.Jackalope.Impl.Subscriptions
{
    public class Subscription : ISubscription
    {
        private string _queueName;
        private IQueueObserver _observer;
        private Task _brokerTask;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _brokerCancelToken;
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
                _brokerTask.Start();
                Starting = false;
                Started = true;
            }
        }

        public void Stop()
        {
            Stopping = true;
            _tokenSource.Cancel();
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
            _tokenSource = new CancellationTokenSource();
            _brokerCancelToken = _tokenSource.Token;
            _brokerCancelToken.Register(_observer.Stop);
            _brokerTask = new Task(() => _observer.Start(_queueName), _brokerCancelToken);
        }
    }
}