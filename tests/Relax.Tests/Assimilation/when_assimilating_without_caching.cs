using System;
using Machine.Specifications;
using StructureMap;
using Symbiote.Core;
using Symbiote.Relax;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Assimilation
{
    [Subject("Assimilation")]
    public class when_assimilating_without_caching
    {
        private Because of = () => RelaxAssimilation.Relax(Assimilate.Core(), x => x.UseDefaults());

        private It should_use_CouchConfiguration_for_ICouchConfiguration = 
            () => ShouldExtensionMethods.ShouldEqual(ObjectFactory
                                       .Container
                                       .Model
                                       .DefaultTypeFor<ICouchConfiguration>(), typeof(CouchConfiguration));

        private It should_use_DocumentRepository_for_IDocumentRepository =
            () => ShouldExtensionMethods.ShouldEqual(ObjectFactory.Model.DefaultTypeFor<IDocumentRepository>(), typeof(DocumentRepository));

        private It should_use_DocumentRepository_for_IDocumentRepository_with_closed_generic =
            () => ShouldExtensionMethods.ShouldEqual(ObjectFactory.Model.DefaultTypeFor(typeof(IDocumentRepository<Guid, string>)), typeof(DocumentRepository));

        private It should_use_DocumentRepository_for_IDocumentRepository_with_open_generic =
            () => ObjectFactory
                      .Model
                      .For<IDocumentRepository<DefaultCouchDocument>>()
                      .PluginType
                      .IsAssignableFrom(typeof (DocumentRepository<DefaultCouchDocument>));
    }
}