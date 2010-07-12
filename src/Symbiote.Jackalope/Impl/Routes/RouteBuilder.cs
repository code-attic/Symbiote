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