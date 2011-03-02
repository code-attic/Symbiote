using System;
using System.Collections.Generic;
using System.Threading;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class Subscription : ISubscription
    {
        private string _queueName;
        private IBroker _broker;
        private ManualResetEvent _reset;

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
                StartBroker();
                Starting = false;
                Started = true;
            }
        }

        public void Stop()
        {
            Stopping = true;
            _reset.Close();
            _broker.Stop();
            _broker = null;
            Stopping = false;
            Stopped = true;
        }

        private void StartBroker()
        {
            var _broker = ObjectFactory.GetInstance<Broker>() as IBroker;
            ThreadPool.QueueUserWorkItem(x => _broker.Start(_queueName, _reset));
        }

        public Subscription(string queueName)
        {
            _queueName = queueName;
            Start();
        }

        protected void InitializeBrokerTask()
        {
            _broker = ObjectFactory.GetInstance<IBroker>();
            _reset = new ManualResetEvent(false);
        }
    }
}