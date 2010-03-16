using MassTransit.StructureMapIntegration;
using MassTransit.Transports.Msmq;

namespace Symbiote.GreyHound
{
    public class GreyHoundRegistry : MassTransitRegistryBase
    {
        private void RegisterEndpoint(GreyHoundEndPointConfigurator endPointConfigurator)
        {
            RegisterServiceBus(
                endPointConfigurator.MsmqEndpointUri,
                c => endPointConfigurator.EndpointConfigurator(c)
                );
        }

        public GreyHoundRegistry(GreyHoundConfigurator configurator) : base(typeof(MsmqEndpoint))
        {
            configurator.EndPoints.ForEach(RegisterEndpoint);
            MsmqEndpointConfigurator.Defaults(c =>
                                                  {
                                                      c.CreateMissingQueues = configurator.CreateMissingQueues;
                                                      c.CreateTransactionalQueues = configurator.CreateTxQueues;
                                                      c.PurgeOnStartup = configurator.PurgeOnStart;
                                                  });
        }
    }
}