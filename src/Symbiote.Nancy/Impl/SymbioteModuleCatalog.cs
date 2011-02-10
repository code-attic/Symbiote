using System.Collections.Generic;
using Nancy;
using Symbiote.Core;

namespace Symbiote.Nancy.Impl
{
    public class SymbioteModuleCatalog
        : INancyModuleCatalog
    {
        public IEnumerable<NancyModule> GetAllModules()
        {
            return Assimilate.GetAllInstancesOf<NancyModule>();
        }

        public NancyModule GetModuleByKey( string moduleKey )
        {
            return Assimilate.GetInstanceOf<NancyModule>( moduleKey );
        }
    }
}