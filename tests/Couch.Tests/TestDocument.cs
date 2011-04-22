using System;
using Symbiote.Core;
using Symbiote.Couch.Impl.Model;

namespace Couch.Tests
{
    public class TestDocument : ComplexCouchDocument<TestDocument, Guid>
    {
        public virtual string Message { get; set; }
        public virtual DateTime CreatedOn { get; set; }

        public TestDocument()
        {
            _documentId = Guid.NewGuid();
            CreatedOn = DateTime.Now;
        }
    }

    public class TestDocumentKeyAccessor : IKeyAccessor<TestDocument>
    {
        public string GetId( TestDocument actor )
        {
            return actor.DocumentId.ToString();
        }

        public void SetId<TKey>( TestDocument actor, TKey key )
        {
            actor.DocumentId = Guid.Parse( key.ToString() );
        }
    }
}