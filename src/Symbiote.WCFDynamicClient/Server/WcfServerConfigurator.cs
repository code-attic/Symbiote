using System;
using System.Collections.Generic;

namespace Symbiote.Wcf.Server
{
    public class WcfServerConfigurator
    {
        public List<IWcfServiceConfiguration> serviceConfigurations { get; protected set; }
        public string BaseAddress { get; set; }

        public WcfServerConfigurator AddService<TService>(Action<WcfServiceConfigurator<TService>> service)
            where TService : class
        {
            var wcfServiceConfigurator = new WcfServiceConfigurator<TService>();
            service(wcfServiceConfigurator);
            serviceConfigurations.Add(wcfServiceConfigurator.GetConfiguration());
            return this;
        }

        public WcfServerConfigurator(string baseAddress)
        {
            BaseAddress = baseAddress;
            serviceConfigurations = new List<IWcfServiceConfiguration>();
        }
    }
}