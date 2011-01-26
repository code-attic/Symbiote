using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Http.Impl.Adapter;
using Symbiote.Http.Impl.Adapter.NetListener;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Config
{
    public class HttpConfigurator
    {
        public HttpListenerConfigurator ListenerConfigurator { get; set; }
        public IRegisterApplication RegisterApplication { get; set; }

        public HttpConfigurator RegisterApplications(Action<IRegisterApplication> register)
        {
            register(RegisterApplication);
            return this;
        }

        public HttpConfigurator ConfigureHttpListener(Action<HttpListenerConfigurator> configurator)
        {
            configurator(ListenerConfigurator);
            Assimilate.Dependencies(x => x.For<IHost>().Use<HttpListenerHost>().AsSingleton());
            return this;
        }

        public HttpConfigurator(HttpListenerConfigurator listenerConfigurator, IRegisterApplication registerApplication)
        {
            ListenerConfigurator = listenerConfigurator;
            RegisterApplication = registerApplication;
        }
    }
}
