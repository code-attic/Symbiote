using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Extensions
{
    public static class MessagingExtensions
    {
        public static string GetRoutingKey(this object  message)
        {
            var routedByKey = message as IRouteByKey;
            return routedByKey == null ? "" : routedByKey.RoutingKey;
        }
    }
}
