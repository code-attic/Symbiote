using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Symbiote.Daemon
{
    public interface IDaemon
    {
        void Start();
        void Stop();
    }
}
