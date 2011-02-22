﻿using Machine.Specifications;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Server
{
    public abstract class with_create_delete_database_command : with_couch_server
    {
        private Establish context = () =>
                                        {
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch");
                                            commandMock.Setup(x => x.Delete(couchUri));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}