using System;
using Symbiote.Core;
using Symbiote.Core.DI;

namespace Symbiote.Daemon
{
    public class DaemonScan : IDefineScanningInstructions
    {
        public Action<IScanInstruction> Scan()
        {
            return scan => scan.AddAllTypesOf<IDaemon>();
        }
    }
}