using System;
using System.IO;

namespace Symbiote.JsonRpc.Impl.Adapters
{
    public interface IHttpRequestAdapter 
    {
        Uri Url { get; }
        Stream InputStream { get; }
    }
}