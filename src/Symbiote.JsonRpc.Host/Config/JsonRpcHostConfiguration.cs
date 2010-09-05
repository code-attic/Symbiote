using System;
using System.Collections.Generic;
using System.Net;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using System.Linq;

namespace Symbiote.JsonRpc.Host.Config
{
    public class JsonRpcHostConfiguration : IJsonRpcHostConfiguration
    {
        protected List<string> Uris { get; set; }

        public AuthenticationSchemes AuthSchemes { get; set; }
        public List<string> HostedUrls
        { 
            get
            {
                Uris = Uris ?? BuildUrls();
                return Uris;
            }
        }
        public List<int> Ports { get; set; }
        public bool UseHttps { get; set; }
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }
        public List<Tuple<Type, Type>> RegisteredServices { get; set; }

        public bool SelfHosted { get; set; }

        public void UseDefaults()
        {
            //set defaults
            SelfHosted = true;
            AuthSchemes = AuthenticationSchemes.Anonymous;
        }

        public List<string> BuildUrls()
        {
            if(Ports.Count == 0)
                throw new AssimilationException("JsonRpc Host can't start up without any assigned ports. Please us the AddPort call during configuration.");

            return Ports.Select(x => @"{0}://localhost:{1}/"
                .AsFormat(UseHttps ? "https" : "http", x))
                .ToList();
        }

        public JsonRpcHostConfiguration()
        {
            Ports = new List<int>();
            RegisteredServices = new List<Tuple<Type, Type>>();
            UseDefaults();
        }
    }
}