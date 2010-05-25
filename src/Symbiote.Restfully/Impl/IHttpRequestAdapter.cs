using System;
using System.IO;

namespace Symbiote.Restfully.Impl
{
    public interface IHttpRequestAdapter 
    {
        Uri Url { get; }
        Stream InputStream { get; }
    }
}