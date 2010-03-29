using Symbiote.Relax;

namespace Relax.Tests.Repository
{
    public class TestDocument : DefaultCouchDocument
    {
        public virtual string Message { get; set; }
    }
}