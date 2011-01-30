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
using Symbiote.Core.DI;
using Symbiote.Http.Config;
using Symbiote.Http.Impl;
using Symbiote.Http.Owin;

namespace Symbiote.Http
{
    public static class HttpHostAssimilation
    {
        public static IAssimilate HttpHost( this IAssimilate assimilate, Action<HttpConfigurator> httpConfigurator )
        {
            Assimilate.Dependencies( x => { SetupDependencies( x ); } );

            var configurator = Assimilate.GetInstanceOf<HttpConfigurator>();
            httpConfigurator( configurator );
            return assimilate;
        }

        public static void SetupDependencies( DependencyConfigurator container )
        {
            var router = new ApplicationRouter();
            container.For<HttpConfigurator>().Use<HttpConfigurator>();
            container.For<HttpListenerConfiguration>().Use<HttpListenerConfiguration>().AsSingleton();
            container.For<IRegisterApplication>().Use( router );
            container.For<IRouteRequest>().Use( router );
        }
    }
}