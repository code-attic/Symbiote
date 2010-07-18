using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text.RegularExpressions;
using Symbiote.Core.Extensions;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceHost : IDisposable
    {
        protected WcfServerConfigurator serverConfiguration;
        protected List<ServiceHost> serviceHosts { get; set; }
        
        public void Start()
        {
            serviceHosts.ForEach(x => x.Open());
        }

        public void Stop()
        {
            serviceHosts.ForEach(x => x.Close());
        }

        public WcfServiceHost(WcfServerConfigurator serverConfiguration)
        {
            this.serverConfiguration = serverConfiguration;
            serviceHosts = serverConfiguration.serviceConfigurations.Select(x => BuildHost(x)).ToList();
        }

        private ServiceHost BuildHost(IWcfServiceConfiguration configuration)
        {
            var serviceUri = configuration.Address;
            var host = new ServiceHost(configuration.ServiceType, new Uri(serverConfiguration.BaseAddress));
            host.AddServiceEndpoint(
                configuration.ContractType, 
                configuration.Binding,
                serviceUri);
            
            if (configuration.EnableHttpMetadataExchange)
            {
                var mexUriString =
                    "{0}/{1}".AsFormat(configuration.MetadataExchangeUri ?? serverConfiguration.BaseAddress, configuration.Address);

                host.Description.Behaviors.Add(
                    new ServiceMetadataBehavior()
                        {
                            HttpGetEnabled = true,
                            HttpGetUrl = new Uri(mexUriString)
                        });

                host.AddServiceEndpoint(typeof(IMetadataExchange), configuration.Binding, "mex");
            }

            host.CloseTimeout = TimeSpan.FromMilliseconds(configuration.Timeout);
            return host;
        }

        public void Dispose()
        {
            Stop();
            serviceHosts.Clear();
        }
    }
}