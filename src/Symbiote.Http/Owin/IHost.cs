using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Http.Owin
{
    public interface IHost
    {
        void Start();
        void Stop();
    }
}
