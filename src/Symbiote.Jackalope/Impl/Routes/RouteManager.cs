using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Jackalope.Impl.Routes
{
    public class RouteManager :
        IRouteManager
    {
        protected ConcurrentDictionary<Type, IRouteMessage> routes { get; set; }

        public IEnumerable<Tuple<string, string>> GetRoutesForMessage(object message)
        {
            return routes
                .Values
                .Where(x => x.AppliesToMessage(message))
                .Select(x => Tuple.Create<string, string>(x.GetExchange(message), x.GetRoutingKey(message)));
        }

        public IRouteManager AddRoute<TMessage>(Action<IDefineRoute<TMessage>> routeDefinition)
        {
            var routeBuilder = new RouteBuilder<TMessage>();
            routeDefinition(routeBuilder);
            routes[typeof(TMessage)] = routeBuilder.Route;
            return this;
        }

        public RouteManager()
        {
            routes = new ConcurrentDictionary<Type,IRouteMessage>();
        }
    }
}