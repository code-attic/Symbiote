using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using StructureMap;
using Symbiote.Core;
using Symbiote.Hibernate.Impl;

namespace Symbiote.Hibernate
{
    public static class HibernateAssimilation
    {
        public static IAssimilate Hibernate(this IAssimilate assimilate, Action<HibernateConfigurator> config)
        {
            var configurator = new HibernateConfigurator(assimilate);
            config(configurator);
            return assimilate;
        }
    }

    public class HibernateConfigurator
    {
        private IAssimilate _assimilate;

        public HibernateConfigurator FromFactory(Action<FluentConfiguration> config)
        {
            var configuration = Fluently.Configure();
            config(configuration);
            _assimilate.Dependencies(x => x.For<ISessionFactory>().Singleton().Use(configuration.BuildSessionFactory()));
            return this;
        }

        public HibernateConfigurator(IAssimilate assimilate)
        {
            _assimilate = assimilate;
            assimilate.Dependencies(x =>
                                        {
                                            x.For<ISessionContext>().Use<InMemoryContext>();
                                            x.For<ISessionManager>().Use<SessionManager>();
                                            x.For<ISessionModule>().Use<SessionModule>();
                                            x.For(typeof (IRepository<>)).Use(typeof (Repository<>));
                                        });
        }
    }
}
