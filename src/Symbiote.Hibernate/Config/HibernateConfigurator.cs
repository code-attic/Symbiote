using System;
using FluentNHibernate.Cfg;
using NHibernate;
using Symbiote.Core;

namespace Symbiote.Hibernate
{
    public class HibernateConfigurator
    {
        private IAssimilate _assimilate;

        public HibernateConfigurator FromFactory( Action<FluentConfiguration> config )
        {
            var configuration = Fluently.Configure();
            config( configuration );
            _assimilate.Dependencies(
                x => x.For<ISessionFactory>().Use( configuration.BuildSessionFactory() ).AsSingleton() );
            return this;
        }
    }
}