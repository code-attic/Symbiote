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
using System.Net;
using Symbiote.Core;

namespace Symbiote.Http.Config
{
    public class HttpServerConfigurator
    {
        private HttpServerConfiguration _configuration;

        public HttpServerConfiguration GetConfiguration()
        {
            return _configuration;
        }

        public HttpServerConfigurator Port(int port)
        {
            _configuration.Port = port;
            return this;
        }

        public HttpServerConfigurator BaseUrl(string url)
        {
            _configuration.BaseUrl = url;
            return this;
        }

        public HttpServerConfigurator UseBasicAuth()
        {
            _configuration.AuthSchemes = AuthenticationSchemes.Basic;
            return this;
        }

        public HttpServerConfigurator UseDigestAuth()
        {
            _configuration.AuthSchemes = AuthenticationSchemes.Digest;
            return this;
        }

        public HttpServerConfigurator HostService<T>()
            where T : class
        {
            Assimilate.Dependencies(
                x => x.Scan(s => s.AddAllTypesOf<T>() ));
            _configuration.RegisteredServices.Add(Tuple.Create(typeof(T), Assimilate.Assimilation.DependencyAdapter.GetDefaultTypeFor<T>()));
            return this;
        }

        public HttpServerConfigurator()
        {
            _configuration = new HttpServerConfiguration();
        }
    }
}