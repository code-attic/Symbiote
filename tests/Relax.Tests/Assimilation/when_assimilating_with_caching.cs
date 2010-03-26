using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using StructureMap;
using Symbiote.Core;
using Symbiote.Relax;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Assimilation
{
    [Subject("Assimilation")]
    public class when_assimilating_with_caching
    {
        private Because of = () => Assimilate.Core().Relax(x => x.UseDefaults().Cache());

        private It should_use_CouchConfiguration_for_ICouchConfiguration =
            () => ObjectFactory
                    .Container
                    .Model
                    .DefaultTypeFor<ICouchConfiguration>()
                    .ShouldEqual(typeof(CouchConfiguration));

        private It should_use_DocumentRepository_for_IDocumentRepository =
            () => ObjectFactory.Model.DefaultTypeFor<IDocumentRepository>().ShouldEqual(typeof(CachedDocumentRepository));

        private It should_use_DocumentRepository_for_IDocumentRepository_with_closed_generic =
            () => ObjectFactory.Model.DefaultTypeFor(typeof(IDocumentRepository<Guid, string>)).ShouldEqual(typeof(CachedDocumentRepository));

        private It should_use_DocumentRepository_for_IDocumentRepository_with_open_generic =
            () => ObjectFactory
                      .Model
                      .For<IDocumentRepository<DefaultCouchDocument>>()
                      .PluginType
                      .IsAssignableFrom(typeof(CachedDocumentRepository<DefaultCouchDocument>));
    }
}
