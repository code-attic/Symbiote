using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.SocketMQ.Impl;

namespace Symbiote.SocketMQ
{
    public static class SocketMQAssimilation
    {
        public static IAssimilate SocketMQ(this IAssimilate assimilate)
        {
            assimilate.Dependencies(x => x.For<IMessageGateway>().Use<MessageGateway>());
            return assimilate;
        }
    }
}
