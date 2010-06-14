using System;
using System.IO;

namespace Symbiote.Restfully.Impl.Adapters
{
    public interface IHttpRequestAdapter 
    {
        Uri Url { get; }
        Stream InputStream { get; }
    }
}