using Machine.Specifications;
using Symbiote.Relax.Impl;

namespace Relax.Tests.URI
{
    public abstract class with_basic_uri
    {
        protected static CouchURI uri;
        private Establish context = () => uri = CouchURI.Build("http", "localhost", 5984, "test");
    }
}