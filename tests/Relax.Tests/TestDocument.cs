using System;
using Symbiote.Relax;

namespace Relax.Tests
{
    public class TestDocument : CouchDocument<TestDocument, Guid, string>
    {
        public virtual string Message { get; set; }

        public TestDocument()
        {
            _documentId = Guid.NewGuid();
        }
    }
}