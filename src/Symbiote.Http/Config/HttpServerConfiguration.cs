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
using System.Security.Cryptography.X509Certificates;
using Symbiote.Http.Impl.Adapter.TcpListener;

namespace Symbiote.Http.Config
{
    public class HttpServerConfiguration : IHttpServerConfiguration
    {
        public int AllowedPendingRequests { get; set; }
        public AuthenticationSchemes AuthSchemes { get; set; }
        public string BaseUrl { get; set; }
        public string DefaultService { get; set; }
        public string DefaultAction { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<Tuple<Type, Type>> RegisteredServices { get; set; }
        public bool UseHttps { get; set; }

        public string X509CertName { get; set; }
        public StoreName X509StoreName { get; set; }
        public StoreLocation X509StoreLocation { get; set; }

        public void UseDefaults()
        {
            //set defaults
            Port = 8420 ;
            AuthSchemes = AuthenticationSchemes.None;
            BaseUrl = "/";
            Host = "localhost";
            AllowedPendingRequests = 10000;
        }

        public HttpServerConfiguration()
        {
            RegisteredServices = new List<Tuple<Type, Type>>();
            UseDefaults();
        }
    }
}