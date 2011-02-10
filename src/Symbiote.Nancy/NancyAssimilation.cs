using System;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Routing;
using Nancy.ViewEngines;
using Symbiote.Core;
using Symbiote.Nancy.Config;
using Symbiote.Nancy.Impl;
using IEnumerableExtenders = Symbiote.Core.Extensions.IEnumerableExtenders;

namespace Symbiote.Nancy
{
    public static class NancyAssimilation
    {
        public static IAssimilate Nancy(this IAssimilate assimilation, Action<NancyConfigurator> configure )
        {
            var configurate = new NancyConfigurator();
            configure( configurate );
            var configuration = configurate.Configuration;
            assimilation
                .Dependencies( x =>
                    {
                        x.For<NancyConfiguration>().Use( configuration );
                        x.For<IRouteResolver>().Use( configuration.RouteResolver ).AsSingleton();
                        x.For<ITemplateEngineSelector>().Use( configuration.TemplateEngineSelector ).AsSingleton();
                        x.For<IModuleKeyGenerator>().Use( configuration.ModuleKeyGenerator ).AsSingleton();
                        x.For<IRouteCache>().Use( configuration.RouteCache ).AsSingleton();
                        x.For<IRouteCacheProvider>().Use( configuration.RouteCacheProvider ).AsSingleton();
                        x.For<IRoutePatternMatcher>().Use( configuration.RoutePatternMatcher ).AsSingleton();
                        x.For<INancyEngine>().Use( configuration.NancyEngine ).AsSingleton();
                        x.For<INancyModuleCatalog>().Use<SymbioteModuleCatalog>();
                        x.For<IViewLocator>().Use<SymbioteViewLocator>();
                    } );

            var moduleKeyGenerator = Assimilate.GetInstanceOf<IModuleKeyGenerator>();

            assimilation
                .Dependencies( x =>
                    {
                        x.Scan( s =>
                            {
                                IEnumerableExtenders.ForEach<Assembly>( AppDomain
                                                            .CurrentDomain
                                                            .GetAssemblies()
                                                            .Where(a =>
                                                                   a.GetReferencedAssemblies()
                                                                       .Any(r => r.FullName.Contains("Nancy"))
                                                                   || a.FullName.Contains("Nancy")), s.Assembly);

                                s.UseNamingStrategyForMultiples( moduleKeyGenerator.GetKeyForModuleType );
                                s.AddAllTypesOf<NancyModule>();       
                            } );
                    } );

            return assimilation;
        }
    }
}