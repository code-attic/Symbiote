using System;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Repository
{
    public abstract class with_get_document_by_key_command : with_document_repository
    {
        protected static Guid id;

        private Establish context = () =>
                                        {
                                            id = Guid.NewGuid();
                                            uri = new CouchURI("http", "localhost", 5984, "testdocument").Key(id);
                                            commandMock
                                                .Setup(x => x.Get(couchUri))
                                                .Returns(@"{{_id : ""{0}"", _rev : ""1"", Message : ""Hello"" }}".AsFormat(id));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}