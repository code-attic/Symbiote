using Nancy.Routing;
using Symbiote.Core;

namespace Symbiote.Nancy.Impl
{
    public class SymbioteRouteCacheProvider
        : IRouteCacheProvider
    {
        public IRouteCache GetCache()
        {
            return Assimilate.GetInstanceOf<IRouteCache>();
        }
    }
}