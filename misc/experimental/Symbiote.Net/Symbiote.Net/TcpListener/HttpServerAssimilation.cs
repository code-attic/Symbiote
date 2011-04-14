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
using Symbiote.Http.Config;

namespace Symbiote.Http.Impl.Adapter.TcpListener
{
    public static class HttpServerAssimilation
    {
        public static IAssimilate HttpServer(this IAssimilate assimilate, Action<HttpServerConfigurator> configure)
        {
            var configurator = new HttpServerConfigurator(new HttpServerConfiguration());
            configure(configurator);

            assimilate
                .Dependencies(x =>
                                  {
                                      var config = configurator.GetConfiguration();
                                      x.For<IHttpServerConfiguration>().Use(config);
                                      x.For<IHttpServer>().Use<HttpServer>();
                                      x.For<IAuthenticationValidator>().Use<WorthlessAuthenticationValidator>();
                                      
                                      if(config.AuthSchemes == AuthenticationSchemes.Basic)
                                      {
                                          x.For<IHttpAuthChallenger>().Use<HttpBasicAuthChallenger>();
                                      }
                                  });

            return assimilate;
        }
    }
}
