using System;
using System.Collections.Generic;
using System.Net;

namespace Symbiote.JsonRpc.Host.Config
{
    public interface IJsonRpcHostConfiguration
    {
        List<string> HostedUrls { get; }
        List<int> Ports { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }
        bool SelfHosted { get; set; }

        List<Tuple<Type, Type>> RegisteredServices { get; set; }

        void UseDefaults();

    }
}