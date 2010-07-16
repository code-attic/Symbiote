using System;
using Symbiote.Core;

namespace Symbiote.JsonRpc.Config
{
    public class HttpServiceHostConfigurator
    {
        private IHttpServiceHostConfiguration _configuration;

        public HttpServiceHostConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public HttpServiceHostConfigurator UseDefaults()
        {
            _configuration.UseDefaults();
            return this;
        }

        public HttpServiceHostConfigurator HostInIIS()
        {
            _configuration.SelfHosted = false;
            return this;
        }

        public IHttpServiceHostConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpServiceHostConfigurator HostService<T>()
            where T : class
        {
            Assimilate.Dependencies(x => x.Scan(s =>
                                                    {
                                                        //s.AssembliesFromApplicationBaseDirectory();
                                                        s.TheCallingAssembly();
                                                        s.AddAllTypesOf<T>();
                                                    }));
            _configuration.RegisteredServices.Add(Tuple.Create(typeof(T), Assimilate.Assimilation.DependencyAdapter.GetDefaultTypeFor<T>()));
            return this;
        }

        public HttpServiceHostConfigurator(IHttpServiceHostConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}