using System;
using System.Collections.Generic;

namespace Symbiote.Jackalope.Impl.Routes
{
    public interface IRouteManager
    {
        IEnumerable<Tuple<string, string>> GetRoutesForMessage(object message);
        IRouteManager AddRoute<TMessage>(Action<IDefineRoute<TMessage>> routeDefinition);
    }
}