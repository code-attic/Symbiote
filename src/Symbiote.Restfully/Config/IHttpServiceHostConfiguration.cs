using System;
using System.Collections.Generic;
using System.Net;

namespace Symbiote.JsonRpc.Config
{
    public interface IHttpServiceHostConfiguration
    {
        IList<string> HostedUrls { get; set; }
        int Port { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }
        bool SelfHosted { get; set; }

        List<Tuple<Type, Type>> RegisteredServices { get; set; }

        void UseDefaults();

    }
}