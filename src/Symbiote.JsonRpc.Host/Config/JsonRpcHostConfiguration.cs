/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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