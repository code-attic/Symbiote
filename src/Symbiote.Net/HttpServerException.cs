using System;

namespace Symbiote.Net
{
    public class HttpServerException : Exception
    {
        public HttpServerException(string message) :base(message) {}
        public HttpServerException(string message, Exception innerException) : base(message, innerException) { }
    }
}