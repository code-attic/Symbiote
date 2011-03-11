using System;
using Nancy;
using Nancy.Bootstrapper;
using Symbiote.Core;
using Symbiote.Core.DI;

namespace Symbiote.Nancy.Config
{
    public class NancyScan : IDefineScanningInstructions
    {
        public Action<IScanInstruction> Scan()
        {
            var moduleKeyGenerator = Assimilate.GetInstanceOf<IModuleKeyGenerator>();
            return scan => 
                       {
                           scan.UseNamingStrategyForMultiples( moduleKeyGenerator.GetKeyForModuleType );
                           scan.AddAllTypesOf<NancyModule>();  
                       };
        }
    }
}