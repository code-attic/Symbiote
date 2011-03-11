using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Routing;
using Nancy.ViewEngines;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Nancy.Impl;

namespace Symbiote.Nancy.Config
{
    public class NancyDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var configurate = new NancyConfigurator();
            var configuration = configurate.Configuration;
            return container => 
                       {
                           container.For<NancyConfiguration>().Use( configuration );
                           container.For<IRouteResolver>().Use( configuration.RouteResolver ).AsSingleton();
                           container.For<ITemplateEngineSelector>().Use( configuration.TemplateEngineSelector ).AsSingleton();
                           container.For<IModuleKeyGenerator>().Use( configuration.ModuleKeyGenerator ).AsSingleton();
                           container.For<IRouteCache>().Use( configuration.RouteCache ).AsSingleton();
                           container.For<IRouteCacheProvider>().Use( configuration.RouteCacheProvider ).AsSingleton();
                           container.For<IRoutePatternMatcher>().Use( configuration.RoutePatternMatcher ).AsSingleton();
                           container.For<INancyEngine>().Use( configuration.NancyEngine ).AsSingleton();
                           container.For<INancyModuleCatalog>().Use<SymbioteModuleCatalog>();
                           container.For<IViewLocator>().Use<SymbioteViewLocator>();
                       };
        }
    }
}