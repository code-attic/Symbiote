using System;
using System.IO;

namespace Symbiote.JsonRpc.Host.Impl.Adapters
{
    public interface IHttpRequestAdapter 
    {
        Uri Url { get; }
        Stream InputStream { get; }
    }
}