using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Symbiote.Wcf.Client
{
    public sealed class ClientShell<T> : ClientBase<T>
        where T : class
    {
        public ClientShell()
        {
        }

        public ClientShell(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ClientShell(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ClientShell(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ClientShell(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
    }
}