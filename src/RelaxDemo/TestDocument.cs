using System;
using Symbiote.Relax;

namespace RelaxDemo
{
    [Serializable]
    public class TestDocument : DefaultCouchDocument
    {
        public virtual string Message { get; set; }

        public TestDocument()
        {
        }

        public TestDocument(string message)
        {
            Message = message;
        }
    }
}