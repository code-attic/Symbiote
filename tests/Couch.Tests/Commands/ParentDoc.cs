using System.Collections.Generic;
using Symbiote.Couch;

namespace Couch.Tests.Commands
{
    public class ParentDoc : CouchDocument
    {
        public virtual string Message { get; set; }
        public virtual List<TestDoc> Children { get; set; }
    }
}