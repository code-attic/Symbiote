using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Redis
{
    public class RiakDocument<T>
    {
        public T Value { get; set; }
        public string ContentType { get; set; }
        public string CharacterSet { get; set; }
        public string ContentEncoding { get; set; }
        public string VTag { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        protected RiakDocument()
        {
            Metadata = new Dictionary<string, object>();
        }



        public static implicit operator T(RiakDocument<T> document)
        {
            return document.Value;
        }
    }
}
