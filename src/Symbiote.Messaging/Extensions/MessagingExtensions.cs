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
    }
}
