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