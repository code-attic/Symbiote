using System;
using Machine.Specifications;
using Relax.Tests.Repository;
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
            () => ObjectFactory
                       .Container
                       .Model
                       .DefaultTypeFor<ICouchConfiguration>()
                       .ShouldEqual(typeof(CouchConfiguration));

        private It should_use_DocumentRepository_for_IDocumentRepository =
            () => ObjectFactory
                      .Container
                      .Model
                      .HasImplementationsFor(typeof(IDocumentRepository))
                      .ShouldBeTrue();
    }
}