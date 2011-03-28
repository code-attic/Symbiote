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
//using Symbiote.Http.NetAdapter.HttpListener;
using Symbiote.Http.NetAdapter.TcpListener;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Config
{
    public class HttpConfigurator
    {
        //public HttpListenerConfigurator ListenerConfigurator { get; set; }
        public HttpServerConfigurator SocketConfigurator { get; set; }
        public IRegisterApplication RegisterApplication { get; set; }
        public HttpWebConfigurator WebConfigurator {get; set; }

        public HttpConfigurator ConfigureWebAppSettings( Action<HttpWebConfigurator> configurator )
        {
            configurator( WebConfigurator );
            return this;
        }

        public HttpConfigurator RegisterApplications( Action<IRegisterApplication> register )
        {
            register( RegisterApplication );
            return this;
        }

        //public HttpConfigurator ConfigureHttpListener( Action<HttpListenerConfigurator> configurator )
        //{
        //    configurator( ListenerConfigurator );
        //    Assimilate.Dependencies( x => 
        //        { 
        //            x.For<IHost>().Use<HttpListenerHost>().AsSingleton();
        //            x.For<HttpListenerConfiguration>().Use( ListenerConfigurator.GetConfiguration() );
        //        } );
        //    return this;
        //}

        public HttpConfigurator ConfigureSocketServer( Action<HttpServerConfigurator> configurator )
        {
            configurator( SocketConfigurator );
            Assimilate.Dependencies( x => 
                { 
                    //x.For<IHost>().Use<SocketServer>().AsSingleton();
                    //x.For<IHttpServerConfiguration>().Use( SocketConfigurator.GetConfiguration() );
                } );
            return this;
        }

        public HttpConfigurator( IRegisterApplication registerApplication )
        {
            //ListenerConfigurator = listenerConfigurator;
            RegisterApplication = registerApplication;
            SocketConfigurator = new HttpServerConfigurator();
            WebConfigurator = new HttpWebConfigurator();

            Assimilate
                .Dependencies( x => 
                {
                    x.For<HttpWebConfiguration>().Use( WebConfigurator.Configuration );
                } );
        }
    }
}
