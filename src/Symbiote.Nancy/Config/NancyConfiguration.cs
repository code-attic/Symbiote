using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Routing;
using Symbiote.Nancy.Impl;

namespace Symbiote.Nancy.Config
{
    public class NancyConfiguration
    {
        public Type NancyEngine { get; set; }
        public Type RouteResolver { get; set; }
        public Type TemplateEngineSelector { get; set; }
        public Type ModuleKeyGenerator { get; set; }
        public Type RouteCache { get; set; }
        public Type RouteCacheProvider { get; set; }
        public Type RoutePatternMatcher { get; set; }

        public NancyConfiguration()
        {
            NancyEngine = typeof( NancyEngine );
            RouteResolver = typeof( DefaultRouteResolver );
            TemplateEngineSelector = typeof( DefaultTemplateEngineSelector );
            ModuleKeyGenerator = typeof( DefaultModuleKeyGenerator );
            RouteCache = typeof( DefaultRouteCache );
            RouteCacheProvider = typeof( SymbioteRouteCacheProvider );
            RoutePatternMatcher = typeof( DefaultRoutePatternMatcher );
        }
    }
}