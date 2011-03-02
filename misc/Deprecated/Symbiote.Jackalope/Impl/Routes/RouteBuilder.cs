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

namespace Symbiote.Jackalope.Impl.Routes
{
    public class RouteBuilder<TMessage> : IDefineRoute<TMessage>
    {
        protected MessageRoute<TMessage> route { get; set; }

        public IRouteMessage Route { get { return route; } }

        public IDefineRoute<TMessage> IncludeChildMessageTypes()
        {
            route.AllowsInheritence = true;
            return this;
        }

        public IDefineRoute<TMessage> RouteBy(Func<TMessage, string> exchangeStrategy)
        {
            route.ExchangeStrategy = exchangeStrategy;
            return this;
        }

        public IDefineRoute<TMessage> SendTo(string exchangeName)
        {
            route.ExchangeStrategy = x => exchangeName;
            return this;
        }

        public IDefineRoute<TMessage> WithRoutingKey(Func<TMessage, string> subjectBuilder)
        {
            route.RoutingKeyBuilder = subjectBuilder;
            return this;
        }

        public IDefineRoute<TMessage> WithRoutingKey(string subject)
        {
            route.RoutingKeyBuilder = x => subject;
            return this;
        }

        public RouteBuilder()
        {
            route = new MessageRoute<TMessage>();
        }
    }
}