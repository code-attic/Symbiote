using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Symbiote.Net
{
    public class HttpServerConfiguration : IHttpServerConfiguration
    {
        public int AllowedPendingRequests { get; set; }
        public AuthenticationSchemes AuthSchemes { get; set; }
        public string BaseUrl { get; set; }
        private const string DEFAULT_HOST_URL = @"http://localhost:8420/";
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }
        public IList<string> HostedUrls { get; set; }
        public int Port { get; set; }
        public List<Tuple<Type, Type>> RegisteredServices { get; set; }
        public bool UseHttps { get; set; }

        public string X509CertName { get; set; }
        public StoreName X509StoreName { get; set; }
        public StoreLocation X509StoreLocation { get; set; }

        public void UseDefaults()
        {
            //set defaults
            Port = 8420;
            AuthSchemes = AuthenticationSchemes.None;
            BaseUrl = DEFAULT_HOST_URL
                .Replace(@"http://","")
                .Split('/')[0]
                .Split(':')[0];
            HostedUrls.Add(DEFAULT_HOST_URL);
            AllowedPendingRequests = 100;
        }

        public HttpServerConfiguration()
        {
            HostedUrls = new List<string>();
            RegisteredServices = new List<Tuple<Type, Type>>();
        }
    }
}