using System;
using System.Linq;
using System.Text;

namespace Symbiote.WCFDynamicClient.Impl
{
    public class DelegateConfigurationStrategy<T> : IServiceClientConfigurationStrategy<T>
    {
        private Action<IServiceConfiguration> _configurationDelegate;

        public void ConfigureServiceClient(IServiceConfiguration configuration)
        {
            _configurationDelegate(configuration);
        }

        public DelegateConfigurationStrategy(Action<IServiceConfiguration> configurationDelegate)
        {
            _configurationDelegate = configurationDelegate;
        }
    }
}
