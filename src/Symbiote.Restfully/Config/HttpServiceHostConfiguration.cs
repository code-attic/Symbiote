using System;
using System.Collections.Generic;
using System.Net;

namespace Symbiote.Restfully.Config
{
    public class HttpServiceHostConfiguration : IHttpServiceHostConfiguration
    {
        public AuthenticationSchemes AuthSchemes { get; set; }
        public IList<string> HostedUrls { get; set; }
        public int Port { get; set; }
        public bool UseHttps { get; set; }
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }
        public List<Tuple<Type, Type>> RegisteredServices { get; set; }

        public bool SelfHosted { get; set; }

        public void UseDefaults()
        {
            //set defaults
            Port = 8420;
            SelfHosted = true;
            AuthSchemes = AuthenticationSchemes.Anonymous;
            HostedUrls.Add(@"http://localhost:8420/");
        }

        public HttpServiceHostConfiguration()
        {
            HostedUrls = new List<string>();
            RegisteredServices = new List<Tuple<Type, Type>>();
        }
    }
}