using System;
using MassTransit.Configuration;
using MassTransit.Services.Routing.Configuration;

namespace Symbiote.GreyHound
{
    public class GreyHoundEndPointConfigurator
    {
        private Action<IServiceBusConfigurator> _endpointConfigurator;
        private GreyHoundConfigurator _serverConfigurator;
        private string _msmqEndpoint;

        public Action<IServiceBusConfigurator> EndpointConfigurator { get { return _endpointConfigurator; } }
        public string MsmqEndpointUri { get { return _msmqEndpoint; } }

        public GreyHoundEndPointConfigurator ForMessageOf<TMessage>(string queueName)
            where TMessage : class
        {
            _msmqEndpoint = _serverConfigurator.GetMsmqEndpoint(queueName);
            _endpointConfigurator = c => c.ConfigureService<RoutingConfigurator>(r => r.Route<TMessage>().To(_msmqEndpoint));
            return this;
        }

        public GreyHoundEndPointConfigurator(GreyHoundConfigurator server)
        {
            _serverConfigurator = server;
        }
    }
}