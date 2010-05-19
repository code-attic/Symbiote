﻿using System;
using StructureMap;

namespace Net.Tests
{
    public class HttpServerConfigurator
    {
        private IHttpServerConfiguration _configuration;

        public HttpServerConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public HttpServerConfigurator UseDefaults()
        {
            _configuration.UseDefaults();
            return this;
        }

        public HttpServerConfigurator BaseUrl(string url)
        {
            _configuration.BaseUrl = url;
            return this;
        }

        public IHttpServerConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpServerConfigurator HostService<T>()
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

        public HttpServerConfigurator(IHttpServerConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}