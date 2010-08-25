using System;
using System.Linq;
using System.Text;
using Symbiote.Core.Reflection;

namespace Symbiote.Jackalope.Impl.Routes
{
    public class MessageRoute<TMessage> : 
        IRouteMessage
    {
        public bool AllowsInheritence { get; set; }

        public bool AppliesToMessage(object message)
        {
            var type = message.GetType();
            var handlesType = typeof(TMessage);
            if(AllowsInheritence)
            {
                return Reflector.GetInheritenceChain(handlesType).Contains(type);
            }
            else
            {
                return type.Equals(handlesType);
            }
        }

        public string GetExchange(object message)
        {
            return ExchangeStrategy((TMessage)message);
        }

        public string GetRoutingKey(object message)
        {
            return RoutingKeyBuilder((TMessage)message);
        }

        public Func<TMessage, string> RoutingKeyBuilder { get; set; }
        public Func<TMessage, string> ExchangeStrategy { get; set; }

        public MessageRoute()
        {
            RoutingKeyBuilder = x => "";
        }
    }
}
