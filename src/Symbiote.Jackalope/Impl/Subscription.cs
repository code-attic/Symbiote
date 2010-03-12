using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class Subscription : ISubscription
    {
        private string _queueName;
        private IBroker _broker;
        private Task _brokerTask;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _brokerCancelToken;

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
            Starting = true;
            _brokerTask.Start();
            Starting = false;
            Started = true;
        }

        public void Stop()
        {
            Stopping = true;
            _tokenSource.Cancel();
            _broker = null;
            Stopping = false;
            Stopped = true;
        }

        public Subscription(string queueName)
        {
            _queueName = queueName;
            _broker = ObjectFactory.GetInstance<Broker>() as IBroker;
            _tokenSource = new CancellationTokenSource();
            _brokerCancelToken = _tokenSource.Token;
            _brokerCancelToken.Register(_broker.Stop);
            _brokerTask = new Task(() => _broker.Start(_queueName), _brokerCancelToken);
        }
    }
}