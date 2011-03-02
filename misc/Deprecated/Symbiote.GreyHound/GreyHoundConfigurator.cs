using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.GreyHound
{
    public class GreyHoundConfigurator
    {
        private string _host = "localhost";
        private string _protocol = "msmq";
        private bool _createMissingQueues = true;
        private bool _purgeOnStart = false;
        private bool _createTxQueues = false;

        private List<GreyHoundEndPointConfigurator> _endPoints = new List<GreyHoundEndPointConfigurator>();
        public List<GreyHoundEndPointConfigurator> EndPoints { get { return _endPoints; } }
        public bool CreateMissingQueues
        {
            get { return _createMissingQueues; }
        }
        public bool PurgeOnStart
        {
            get { return _purgeOnStart; }
        }
        public bool CreateTxQueues
        {
            get { return _createTxQueues; }
        }
        
        public GreyHoundConfigurator Host(string hostName)
        {
            _host = hostName;
            return this;
        }
        public GreyHoundConfigurator OnlyUseExistingQueues()
        {
            _createMissingQueues = false;
            return this;
        }
        public GreyHoundConfigurator AutoCreateTxQueues()
        {
            _createTxQueues = true;
            return this;
        }
        public GreyHoundConfigurator PurgeQueues()
        {
            _purgeOnStart = true;
            return this;
        }
        
        public string GetMsmqEndpoint(string queueName)
        {
            return "{0}://{1}/{2}".AsFormat(_protocol, _host, queueName);
        }

        public GreyHoundConfigurator AddEndpoint(Action<GreyHoundEndPointConfigurator> configure)
        {
            var configurator = new GreyHoundEndPointConfigurator(this);
            configure(configurator);
            _endPoints.Add(configurator);
            return this;
        }
    }
}