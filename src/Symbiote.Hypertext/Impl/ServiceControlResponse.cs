using System;
using System.Collections.Generic;

namespace Symbiote.Telepathy
{
    public class ServiceControlResponse
    {
        public virtual string Message { get; set; }
        public virtual bool ExceptionOccurred { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<Exception> Exceptions { get; set; }
    }
}