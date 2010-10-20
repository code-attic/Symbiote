/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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