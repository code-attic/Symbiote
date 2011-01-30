// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using Symbiote.Core;
using Symbiote.Http.Impl.Adapter.NetListener;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Config
{
    public class HttpConfigurator
    {
        public HttpListenerConfigurator ListenerConfigurator { get; set; }
        public IRegisterApplication RegisterApplication { get; set; }

        public HttpConfigurator RegisterApplications( Action<IRegisterApplication> register )
        {
            register( RegisterApplication );
            return this;
        }

        public HttpConfigurator ConfigureHttpListener( Action<HttpListenerConfigurator> configurator )
        {
            configurator( ListenerConfigurator );
            Assimilate.Dependencies( x => x.For<IHost>().Use<HttpListenerHost>().AsSingleton() );
            return this;
        }

        public HttpConfigurator( HttpListenerConfigurator listenerConfigurator, IRegisterApplication registerApplication )
        {
            ListenerConfigurator = listenerConfigurator;
            RegisterApplication = registerApplication;
        }
    }
}