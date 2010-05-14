using System;
using System.Collections.Generic;
using System.Net;

namespace Symbiote.Restfully.Impl
{
    public interface IHttpServerConfiguration
    {
        IList<string> HostedUrls { get; set; }
        int Port { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }

        List<Tuple<Type, Type>> RegisteredServices { get; set; }

        void UseDefaults();

    }
}