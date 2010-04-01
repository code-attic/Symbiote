using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Symbiote.Core
{
    public class AssimilationException : Exception
    {
        public AssimilationException(string message) : base(message)
        {
        }
    }
}
