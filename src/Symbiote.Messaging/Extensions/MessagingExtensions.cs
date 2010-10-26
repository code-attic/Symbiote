using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Extensions
{
    public static class MessagingExtensions
    {
        public static bool IsOfType<T>(this object message)
        {
            var messageType = typeof(T);
            var compareTo = message.GetType();
            return compareTo.IsAssignableFrom(messageType) || messageType.IsAssignableFrom(compareTo);
        }

        public static string GetRoutingKey(this object message)
        {
            var routedByKey = message as IRouteByKey;
            return routedByKey == null ? "" : routedByKey.RoutingKey;
        }

        public static string GetCorrelationId(this object message)
        {
            var correlates = message as ICorrelate;
            return correlates == null ? null : correlates.CorrelationId;
        }
    }
}
