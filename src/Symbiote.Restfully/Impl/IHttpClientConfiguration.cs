using System;
using System.Collections.Generic;

namespace Symbiote.Restfully.Impl
{
    public interface IHttpClientConfiguration
    {
        string ServerUrl { get; set; }
        int Timeout { get; set; }
    }
}