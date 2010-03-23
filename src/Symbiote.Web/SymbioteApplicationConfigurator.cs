using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spark;
using Spark.Web.Mvc;
using StructureMap;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Hibernate;
using Symbiote.Jackalope.Impl;
using Symbiote.Web.Impl;

namespace Symbiote.Web
{
    public class TypeList : List<Type>
    {
        public TypeList AddType<T>()
        {
            this.Add(typeof(T));
            return this;
        }

        public TypeList AddType(Type type)
        {
            this.Add(type);
            return this;
        }
    }

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
            var newList = new TypeList();
            RegisterSpark(newList);
            return this;
        }

        public SymbioteApplicationConfigurator UseSparkViews(TypeList list)
        {
            RegisterSpark(list);
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

        private void RegisterSpark(TypeList list)
        {
            _assimilate
                .Dependencies(x => x.Scan(s =>
                                              {
                                                  s.TheCallingAssembly();
                                                  var assemblies = AppDomain
                                                      .CurrentDomain
                                                      .GetAssemblies()
                                                      .Where(a => a.GetReferencedAssemblies().Any(r => r.FullName.Contains("System.Web.Mvc")));
                                                  assemblies
                                                      .ForEach(s.Assembly);
                                                  s.AddAllTypesOf<Controller>();
                                                  s.Include(i => i.IsSubclassOf(typeof(Controller)));
                                              }));
            var batch = new SparkBatchDescriptor();
            var allInstances = ObjectFactory
                .GetAllInstances<Controller>();

            var types = allInstances.Select(x => x.GetType());

            allInstances
                .ForEach(x => batch.For(x.GetType()));

            var settings = new SparkSettings()
                .SetDebug(true)
                .SetAutomaticEncoding(true)
                .AddNamespace("System")
                .AddNamespace("System.Collections.Generic")
                .AddNamespace("System.Linq")
                .AddNamespace("System.Web.Mvc")
                .AddNamespace("System.Web.Mvc.Html");

            list
                .ForEach(x => settings.AddNamespace(x.Namespace));

            var sparkViewFactory = new SparkViewFactory(settings);
            //sparkViewFactory.Precompile(batch);
            ViewEngines.Engines.Add(sparkViewFactory);
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
            _assimilate
                .Dependencies(x => x.For<ISubscription>().Use<NullSubscription>());
        }
    }
}