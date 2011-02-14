using Symbiote.Couch;

namespace Couch.Tests.Commands
{
    public class TestDoc : CouchDocument
    {
        public virtual string Message { get; set; }
    }
}