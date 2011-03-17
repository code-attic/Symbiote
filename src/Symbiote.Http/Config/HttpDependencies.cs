using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Http.Owin;
using Symbiote.Http.Owin.Impl;
using Symbiote.Http.ViewAdapter;
using Symbiote.Http.ViewAdapter.NHamlAdapter;

namespace Symbiote.Http.Config
{
    public class HttpDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var router = new ApplicationRouter();
            return container => 
                       {
                           container.For<HttpConfigurator>().Use<HttpConfigurator>();
                           container.For<IRegisterApplication>().Use( router );
                           container.For<IRouteRequest>().Use( router );
                           container.For<IViewEngine>().Use<NHamlEngine>().AsSingleton();
                       };
        }
    }
}