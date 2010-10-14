using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging
{
    public interface IRouteByKey
    {
        string RoutingKey { get; set; }
    }
}
