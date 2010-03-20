using System.Web;
using System.Web.Mvc;
using Spark;
using Spark.Web.Mvc;
using Symbiote.Core;
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

        public SymbioteApplicationConfigurator UseSparkViews()
        {
            RegisterSpark();
            return this;
        }

        public SymbioteApplicationConfigurator SupplyControllerDependencies()
        {
            UseStructureMapFactory();
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

        private void RegisterSpark()
        {
            var settings = new SparkSettings()
                .SetDebug(true)
                .AddNamespace("System")
                .AddNamespace("System.Collections.Generic")
                .AddNamespace("System.Linq")
                .AddNamespace("System.Web.Mvc");

            ViewEngines.Engines.Add(new SparkViewFactory(settings));
        }

        private void UseStructureMapFactory()
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