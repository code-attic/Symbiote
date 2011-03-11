using Nancy;
using Nancy.Bootstrapper;
using Nancy.Routing;

namespace Symbiote.Nancy.Config
{
    public class NancyConfigurator
    {
        public NancyConfiguration Configuration { get; set;}

        public NancyConfigurator UseViewEngine<TEngine>()
            where TEngine : INancyEngine
        {
            Configuration.NancyEngine = typeof( TEngine );
            return this;
        }

        public NancyConfigurator UseRouteResolver<TResolver>()
            where TResolver : IRouteResolver
        {
            Configuration.RouteResolver = typeof(TResolver);
            return this;
        }

        public NancyConfigurator UseTemplateSelector<TSelector>()
            where TSelector : ITemplateEngineSelector
        {
            Configuration.TemplateEngineSelector = typeof(TSelector);
            return this;
        }

        public NancyConfigurator UseModuleKeyGenerator<TGenerator>()
            where TGenerator : IModuleKeyGenerator
        {
            Configuration.ModuleKeyGenerator = typeof(TGenerator);
            return this;
        }

        public NancyConfigurator UseRouteCache<TRouteCache>()
            where TRouteCache : IRouteCache
        {
            Configuration.RouteCache = typeof(TRouteCache);
            return this;
        }

        public NancyConfigurator UseRouteCacheProvider<TCacheProvider>()
            where TCacheProvider : IRouteCacheProvider
        {
            Configuration.RouteCacheProvider = typeof(TCacheProvider);
            return this;
        }

        public NancyConfigurator UseRoutePatternMatcher<TPatternMatcher>()
            where TPatternMatcher : IRoutePatternMatcher
        {
            Configuration.RoutePatternMatcher = typeof(TPatternMatcher);
            return this;
        }

        public NancyConfigurator()
        {
            Configuration = new NancyConfiguration();
        }
    }
}