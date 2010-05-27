using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Symbiote.Net
{
    public interface IHttpServerConfiguration
    {
        string BaseUrl { get; set; }
        IList<string> HostedUrls { get; set; }
        int Port { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }
        List<Tuple<Type, Type>> RegisteredServices { get; set; }
        void UseDefaults();
        bool UseHttps { get; set; }
        string X509CertName { get; set; }
        StoreName X509StoreName { get; set; }
        StoreLocation X509StoreLocation { get; set; }
    }
}