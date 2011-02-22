using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene.Config
{
    public class LuceneDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var configurator = new LuceneConfigurator();
            var configuration = configurator.GetConfiguration();
            return container =>
                       {
                           container.For<ILuceneConfiguration>().Use( configuration );
                           container.For<ILuceneServiceFactory>().Use<LuceneServiceFactory>().AsSingleton();
                           container.For<IDocumentQueue>().Use<DocumentQueue>().AsSingleton();
                           container.For<BaseIndexingObserver>().Use<LuceneIndexingObserver>();
                           container.For<BaseSearchProvider>().Use<LuceneSearchProvider>();
                       };
        }
    }
}