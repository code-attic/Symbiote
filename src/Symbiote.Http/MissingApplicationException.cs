using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Http
{
    public class MissingApplicationException : Exception
    {
        public MissingApplicationException(string message) : base(message)
        {
        }
    }
}
