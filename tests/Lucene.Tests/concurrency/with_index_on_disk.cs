﻿using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Lucene;
using Symbiote.StructureMap;

namespace Lucene.Tests.concurrency
{
    public abstract class with_index_on_disk
    {
        protected static ILuceneServiceFactory factory;
        protected static ILuceneSearchProvider searchProvider;

        private Establish context = () =>
                                        {
                                            Assimilate.Core<StructureMapAdapter>().Lucene();
                                            factory = Assimilate.GetInstanceOf<ILuceneServiceFactory>();
                                            searchProvider = factory.GetSearchProviderForIndex("default");
                                        };
    }
}