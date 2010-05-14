using System;
using System.ServiceModel.Channels;

namespace Symbiote.Wcf
{
    public interface IWcfServiceConfiguration
    {
        string Address { get; set; }
        Binding Binding { get; set; }
        int Timeout { get; set; }
        bool EnableHttpMetadataExchange { get; set; }
        Type ServiceType { get; }
        Type ContractType { get; }

        void UseDefaults();
    }
}