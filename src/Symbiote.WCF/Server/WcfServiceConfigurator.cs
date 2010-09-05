using System;
using System.ServiceModel.Channels;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceConfigurator<TContract>
        where TContract : class
    {
        protected IWcfServiceConfiguration<TContract> configuration { get; set; }

        public IWcfServiceConfiguration<TContract> GetConfiguration()
        {
            return configuration;
        }

        public WcfServiceConfigurator<TContract> Binding(Binding binding)
        {
            configuration.Binding = binding;
            return this;
        }

        public WcfServiceConfigurator<TContract> DisableMetadataExchange()
        {
            configuration.EnableHttpMetadataExchange = false;
            return this;
        }

        public WcfServiceConfigurator<TContract> MexAddress(string metadataExchangeUri)
        {
            configuration.MetadataExchangeUri = metadataExchangeUri;
            return this;
        }

        public WcfServiceConfigurator<TContract> Timeout(TimeSpan timespan)
        {
            configuration.Timeout = timespan.Milliseconds;
            return this;
        }

        public WcfServiceConfigurator()
        {
            configuration = new WcfServiceConfiguration<TContract>();
        }
    }
}