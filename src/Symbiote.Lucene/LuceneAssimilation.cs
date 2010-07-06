using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Lucene.Config;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene
{
    public static class LuceneAssimilation
    {
        public static IAssimilate Lucene(this IAssimilate assimilate, Action<LuceneConfigurator> config)
        {
            var configurator = new LuceneConfigurator();
            config(configurator);
            var configuration = configurator.GetConfiguration();

            assimilate
                .Dependencies(x =>
                {
                    x.For<ILuceneConfiguration>().Use(configuration).AsSingleton();
                    x.For<ILuceneServiceFactory>().Use<LuceneServiceFactory>().AsSingleton();
                    x.For<BaseIndexingObserver>().Use<LuceneIndexingObserver>();
                    x.For<BaseSearchProvider>().Use<LuceneSearchProvider>();
                });

            return assimilate;
        }
    }
}
