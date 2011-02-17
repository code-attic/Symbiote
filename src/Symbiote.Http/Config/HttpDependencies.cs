using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Http.Impl;
using Symbiote.Http.Impl.ViewProvider;
using Symbiote.Http.Impl.ViewProvider.NHamlAdapter;
using Symbiote.Http.Owin;

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