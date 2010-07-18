using System;
using Symbiote.Core;

namespace Symbiote.JsonRpc.Host.Config
{
    public class JsonRpcHostConfigurator
    {
        private IJsonRpcHostConfiguration _configuration;

        public JsonRpcHostConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public JsonRpcHostConfigurator UseDefaults()
        {
            _configuration.UseDefaults();
            return this;
        }

        public JsonRpcHostConfigurator HostInIIS()
        {
            _configuration.SelfHosted = false;
            return this;
        }

        public IJsonRpcHostConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public JsonRpcHostConfigurator HostService<T>()
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

        public JsonRpcHostConfigurator(IJsonRpcHostConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}