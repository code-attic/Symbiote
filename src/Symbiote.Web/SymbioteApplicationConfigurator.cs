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

using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Spark;
using Spark.Web.Mvc;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Hibernate;
using Symbiote.Web.Impl;

namespace Symbiote.Web
{
    public class SymbioteApplicationConfigurator
    {
        private IAssimilate _assimilate;

        public SymbioteApplicationConfigurator AddModule(IHttpModule module)
        {
            _assimilate.Dependencies(x => x.For<IHttpModule>().Add(module));
            return this;
        }

        public SymbioteApplicationConfigurator SupplyControllerDependencies()
        {
            UseSymbioteControllerFactory();
            return this;
        }

        public SymbioteApplicationConfigurator UseNHibernate()
        {
            _assimilate.Dependencies(x =>
                                         {
                                             x.For<ISessionContext>().Use<HttpContextAdapter>();
                                             x.For<IHttpModule>().Add<SessionPerViewModule>();
                                         });
            return this;
        }

        private void UseSymbioteControllerFactory()
        {
            ControllerBuilder
                .Current
                .SetControllerFactory(
                    new SymbioteControllerFactory()
                );
        }

        public SymbioteApplicationConfigurator(IAssimilate assimilate)
        {
            _assimilate = assimilate;
        }
    }
}