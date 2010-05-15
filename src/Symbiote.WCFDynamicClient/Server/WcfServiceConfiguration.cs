using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Symbiote.Core.Extensions;

namespace Symbiote.Wcf.Server
{
    public class WcfServiceConfiguration<TContract> : IWcfServiceConfiguration<TContract>
        where TContract : class 
    {
        public string Address { get; set; }
        public Binding Binding { get; set; }
        public int Timeout { get; set; }
        public bool EnableHttpMetadataExchange { get; set; }
        public Type ServiceType
        {
            get { return typeof (TContract); }
        }

        public Type ContractType
        {
            get { return typeof (TContract).GetInterfaces().First(); }
        }

        public void UseDefaults()
        {
            Address = "{0}".AsFormat(typeof (TContract).Name);
            Binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            Timeout = 600;
            EnableHttpMetadataExchange = true;
        }
    }
}