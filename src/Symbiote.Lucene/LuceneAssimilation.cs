using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;

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
                    x.For<ILuceneConfiguration>().Singleton().Use(configuration);
                    x.For<LuceneServiceFactory>().Singleton().Use<LuceneServiceFactory>();
                });

            return assimilate;
        }
    }
}
