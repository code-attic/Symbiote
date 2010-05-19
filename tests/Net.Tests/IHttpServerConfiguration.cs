using System;
using System.Collections.Generic;
using System.Net;

namespace Net.Tests
{
    public interface IHttpServerConfiguration
    {
        AuthenticationSchemes AuthSchemes { get; set; }
        string BaseUrl { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }
        IList<string> HostedUrls { get; set; }
        int AllowedPendingRequests { get; set; }
        int Port { get; set; }
        List<Tuple<Type, Type>> RegisteredServices { get; set; }
        void UseDefaults();

    }
}