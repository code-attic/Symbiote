using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Symbiote.Net
{
    public interface IHttpServer :
        IObservable<HttpContext>, 
        IDisposable
    {
        void Start();
        void Stop();
    }
}
