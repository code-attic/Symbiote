using System.Collections.Generic;
using System.Net;

namespace Symbiote.Restfully
{
    public interface IHttpServerConfiguration
    {
        IList<string> HostedUrls { get; set; }
        int Port { get; set; }
        AuthenticationSchemes AuthSchemes { get; set; }
        string DefaultService { get; set; }
        string DefaultAction { get; set; }

        void UseDefaults();
    }
}