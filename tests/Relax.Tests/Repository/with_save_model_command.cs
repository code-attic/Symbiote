using System;
using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Repository
{
    public abstract class with_save_model_command : with_test_document
    {
        protected static Guid id;
        protected static string originalDocument;

        private Establish context = () =>
                                        {
                                            id = Guid.NewGuid();
                                            document = new TestDocument()
                                                           {
                                                               Id = id,
                                                               Message = "Hello",
                                                               Revision = "2"
                                                           };
                                            originalDocument = document.ToJson();

                                            uri = new CouchURI("http", "localhost", 5984, "testdocument")
                                                .Key(id);
                                            commandMock.Setup(x => x.Put(couchUri, document.ToJson()))
                                                .Returns("{{ ok : \"true\", id : \"{0}\", rev : \"3\" }}".AsFormat(id));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}