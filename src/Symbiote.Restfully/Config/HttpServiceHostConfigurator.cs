using System;
using StructureMap;

namespace Symbiote.Restfully.Config
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
            ObjectFactory.Configure(x => x.Scan(s =>
                                                    {
                                                        //s.AssembliesFromApplicationBaseDirectory();
                                                        s.TheCallingAssembly();
                                                        s.SingleImplementationsOfInterface();
                                                        s.AddAllTypesOf<T>();
                                                    }));
            _configuration.RegisteredServices.Add(Tuple.Create(typeof(T), ObjectFactory.Model.DefaultTypeFor<T>()));
            return this;
        }

        public HttpServiceHostConfigurator(IHttpServiceHostConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}