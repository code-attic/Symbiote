using System;
using Symbiote.Core;
using Symbiote.Core.DI;

namespace Symbiote.Mikado.Configuration
{
    public class MikadoScan : IProvideScan
    {
        public Action<IScanInstruction> Scan()
        {
            return scan =>
                       {
                           scan.ConnectImplementationsToTypesClosing(typeof(IRule<>));
                       };
        }
    }
}