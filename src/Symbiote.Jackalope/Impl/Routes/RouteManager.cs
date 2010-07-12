using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Jackalope.Impl.Routes
{
    public class RouteManager :
        IRouteManager
    {
        protected ConcurrentBag<IRouteMessage> routes { get; set; }

        public IEnumerable<Tuple<string, string>> GetRoutesForMessage(object message)
        {
            return routes
                .Where(x => x.AppliesToMessage(message))
                .Select(x => Tuple.Create<string, string>(x.GetExchange(message), x.GetRoutingKey(message)));
        }

        public IRouteManager AddRoute<TMessage>(Action<IDefineRoute<TMessage>> routeDefinition)
        {
            var routeBuilder = new RouteBuilder<TMessage>();
            routeDefinition(routeBuilder);
            routes.Add(routeBuilder.Route);
            return this;
        }

        public RouteManager()
        {
            routes = new ConcurrentBag<IRouteMessage>();
        }
    }
}