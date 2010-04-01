using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Moq;
using Relax.Tests.Repository;
using StructureMap;
using Symbiote.Core;
using Symbiote.Eidetic;
using Symbiote.Relax;
using Symbiote.Relax.Impl;
using It = Machine.Specifications.It;

namespace Relax.Tests.Assimilation
{

    [Subject("Assimilation")]
    public class when_assimilating_with_caching
    {
        private Because of = () => 
        {
            var rememberMock = new Mock<IRemember>().Object;
            Assimilate
                .Core()
                .Dependencies(x => x.For<IRemember>().Use(rememberMock))
                .Relax(x => x.UseDefaults().Cache());
        };

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
                      .HasImplementationsFor(typeof (IDocumentRepository))
                      .ShouldBeTrue();
    }
}
