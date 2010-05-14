using System;
using System.Collections.Generic;
using System.Net;

namespace Symbiote.Restfully.Impl
{
    public class HttpServerConfiguration : IHttpServerConfiguration
    {
        public AuthenticationSchemes AuthSchemes { get; set; }
        public IList<string> HostedUrls { get; set; }
        public int Port { get; set; }
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }
        public List<Tuple<Type, Type>> RegisteredServices { get; set; }

        public void UseDefaults()
        {
            //set defaults
            Port = 8420;
            AuthSchemes = AuthenticationSchemes.Anonymous;
            HostedUrls.Add(@"http://localhost:8420/");
        }

        public HttpServerConfiguration()
        {
            HostedUrls = new List<string>();
            RegisteredServices = new List<Tuple<Type, Type>>();
        }
    }
}